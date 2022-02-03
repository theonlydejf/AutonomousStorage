package team.hobbyrobot.subos.net.api;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.Closeable;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.io.PrintWriter;
import java.net.*;
import java.util.ArrayList;
import java.util.Hashtable;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import lejos.hardware.Sound;
import lejos.utility.Stopwatch;
import team.hobbyrobot.subos.SubOSController;
import team.hobbyrobot.subos.errorhandling.ErrorLogging;
import team.hobbyrobot.subos.logging.Logger;
import team.hobbyrobot.subos.logging.VerbosityLogger;
import team.hobbyrobot.subos.net.ClientRegisterer;
import team.hobbyrobot.subos.net.api.exceptions.RequestGeneralException;
import team.hobbyrobot.subos.net.api.exceptions.RequestParamsException;
import team.hobbyrobot.subos.net.api.exceptions.UnknownRequestException;
import team.hobbyrobot.tdn.base.TDNParsers;
import team.hobbyrobot.tdn.core.TDNRoot;
import team.hobbyrobot.tdn.core.TDNValue;

public class TDNAPIServer implements ClientRegisterer, Runnable
{
	// - Constants for parsing requests - //
	public static final String SERVICE_KEYWORD = "service";
	public static final String REQUEST_KEYWORD = "request";
	public static final String PARAMS_KEYWORD = "params";

	// - Constants for parsing responses - //
	public static final String ERROR_CODE_KEYWORD = "error-code";
	public static final String ERROR_DETAILS_KEYWORD = "details";
	public static final String DATA_KEYWORD = "data";

	// - Variables for TCP server - //
	/** Port on which the api server is listening */
	private int port;
	/** The instance of server on which the api is listening */
	private ServerSocket server;
	/** 
	 *  Flag which describes whether the api server should be listening (doesn't determin if 
	 *  the server is listening) 
	 */
	private boolean shouldBeRunning = false;
	/** Maximum amount of clients, that can be connected to the server at once (-1 for no limit) */
	private int maxClients = -1;
	
	// - Variables for client registering - //
	/** Thread, on which the api is accepting clients */
	private Thread clientRegistererThread = null;
	/** List of all connected clients */
	private List<APIEndpoint> clients;
	/** 
	 *  Flag which describes whether the api server should try to accept new client
	 *  (doesn't determin wheter the server is accepting nor whether the registerer reached
	 *  client limit) 
	 */
	private boolean acceptingClients = false;

	// - Variables for service support - //
	/** Map containing all registered services and their corresponding names */
	private Map<String, Service> services;

	// - Loggers - //
	/** Logger used to log any info */
	private VerbosityLogger infoLogger;
	/** Logger used to log any error */
	private Logger errorLogger;

	public TDNAPIServer(int port, Logger infoLogger, Logger errorLogger)
	{
		this.port = port;
		this.infoLogger = new VerbosityLogger(infoLogger);
		this.errorLogger = errorLogger;
		services = new Hashtable<String, Service>();
		clients = new ArrayList<APIEndpoint>();
	}

	public TDNAPIServer(int port, Logger logger)
	{
		this(port, logger, null);
	}

	private void logError(String msg)
	{
		if (errorLogger == null)
			infoLogger.priorityLog(msg);
		else
			errorLogger.log(msg);
	}

	@Override
	public void run()
	{
		if (shouldBeRunning)
			return;
		shouldBeRunning = true;
		
		
		while (shouldBeRunning)
		{
			if (clients.size() <= 0)
				continue;

			APIEndpoint client = null;
			try
			{
				Stopwatch sw = new Stopwatch();
				LinkedList<APIEndpoint> clientsToRemove = new LinkedList<APIEndpoint>();
				synchronized (clients)
				{
					for (APIEndpoint _client : clients)
					{
						if (_client.client.isClosed())
							clientsToRemove.add(_client);
						if (_client.inputStream.available() > 0)
						{
							client = _client;
							sw.reset();
							break;
						}
					}

					for (APIEndpoint disconnectedClient : clientsToRemove)
					{
						clients.remove(disconnectedClient);
						infoLogger.log("Client disconnected from API", VerbosityLogger.OVERVIEW);
					}
				}
				if (client == null)
					continue;
				Stopwatch partSw = new Stopwatch();
				TDNRoot request = TDNRoot.readFromStream(client.reader);
				int parseTook = partSw.elapsed();
				partSw.reset();
				TDNRoot response = processRequest(request);
				int responseTook = partSw.elapsed();
				response.insertValue("took", new TDNValue(sw.elapsed(), TDNParsers.INTEGER));
				response.insertValue("parseTook", new TDNValue(parseTook, TDNParsers.INTEGER));
				response.insertValue("responseTook", new TDNValue(responseTook, TDNParsers.INTEGER));

				response.writeToStream(client.writer);
			}
			catch (IOException ex)
			{
				logError("TDNAPIServer - exception was thrown when talking to a client; EXCEPTION: "
					+ Logger.getExceptionInfo(ex));
			}
		}
	}

	private synchronized TDNRoot processRequest(TDNRoot request)
	{
		TDNValue serviceStr = request.get(SERVICE_KEYWORD);
		if (serviceStr == null || !serviceStr.parser().typeKey().equals(TDNParsers.STRING.typeKey()))
			return ResponseFactory.createExceptionResponse(ErrorCode.UNKNOWN_SERVICE,
				"Unable to find service in the root");

		Service service = services.get((String) serviceStr.value);
		if (service == null)
			return ResponseFactory.createExceptionResponse(ErrorCode.UNKNOWN_SERVICE,
				"Service \"" + (String) serviceStr.value + "\" is not registered");

		TDNValue requestName = request.get(REQUEST_KEYWORD);
		if (requestName == null || !requestName.parser().typeKey().equals(TDNParsers.STRING.typeKey()))
			return ResponseFactory.createExceptionResponse(ErrorCode.UNKNOWN_REQUEST,
				"Unable to find request in the root");

		TDNValue params = request.get(PARAMS_KEYWORD);
		if (params == null || !params.parser().typeKey().equals(TDNParsers.ROOT.typeKey()))
			return ResponseFactory.createExceptionResponse(ErrorCode.PARAMS_ERROR, "Unable to find params in the root");

		try
		{
			return ResponseFactory
				.createSuccessResponse(service.processRequest((String) requestName.value, (TDNRoot) params.value));
		}
		catch (UnknownRequestException e)
		{
			return ResponseFactory.createExceptionResponse(ErrorCode.UNKNOWN_REQUEST,
				"Unknown request: " + (String) requestName.value);
		}
		catch (RequestParamsException e)
		{
			return ResponseFactory.createExceptionResponse(ErrorCode.PARAMS_ERROR,
				e.message + "; Bad params: " + joinStr(", ", e.badParams));
		}
		catch (RequestGeneralException e)
		{
			return ResponseFactory.createExceptionResponse(ErrorCode.GENERAL_EXCEPTION, e.details);
		}
	}

	private static String joinStr(String separator, String[] input)
	{

		if (input == null || input.length <= 0)
			return "";

		StringBuilder sb = new StringBuilder();

		for (int i = 0; i < input.length; i++)
		{

			sb.append(input[i]);

			// if not the last item
			if (i != input.length - 1)
			{
				sb.append(separator);
			}

		}

		return sb.toString();

	}

	//TODO
	public synchronized void stop()
	{

	}

	@Override
	public void startRegisteringClients() throws IOException
	{
		if (isRegisteringClients())
			return;

		acceptingClients = true;
		clients = new LinkedList<APIEndpoint>();
		server = new ServerSocket(getPort());

		clientRegistererThread = new Thread()
		{
			public void run()
			{
				while (acceptingClients)
				{
					if(getMaxClients() > -1 && clients.size() >= getMaxClients())
						continue;
					
					acceptClient();
				}
			}
		};
		clientRegistererThread.setDaemon(true);
		clientRegistererThread.setPriority(Thread.MIN_PRIORITY);
		clientRegistererThread.start();
	}

	public synchronized void registerService(String serviceKey, Service service)
	{
		services.put(serviceKey, service);
	}

	private void acceptClient()
	{
		Socket client = null;
		try
		{
			Sound.beep();
			client = server.accept();
			Sound.twoBeeps();
		}
		catch (SocketException ex)
		{
			infoLogger.log("SocketException was thrown when waiting for a clinet"
				+ " in ASCSVehicleAPIServer. This may indicate that the " + "registering server was closed. EXCEPTION: "
				+ Logger.getExceptionInfo(ex), VerbosityLogger.DEFAULT);
		}
		catch (IOException ex)
		{
			logError("IOException was thrown when waiting for a clinet" + " in ASCSVehicleAPIServer. EXCEPTION: "
				+ Logger.getExceptionInfo(ex));
		}
		if (client == null) 
			return;

		try
		{
			synchronized(clients)
			{
				clients.add(new APIEndpoint(client));				
			}
			infoLogger.log("client registered", VerbosityLogger.DETAILED_OVERVIEW);
		}
		catch (IOException ex)
		{
			logError("IOException was thrown when connecting to a clinet" + " in ASCSVehicleAPIServer. EXCEPTION: "
				+ Logger.getExceptionInfo(ex));
		}
	}

	@Override
	public synchronized void stopRegisteringClients() throws IOException
	{
		if (!isRegisteringClients())
			return;

		acceptingClients = false;
		server.close();
		server = null;
	}

	@Override
	public synchronized boolean isRegisteringClients()
	{
		return clientRegistererThread != null && clientRegistererThread.isAlive();
	}

	@Override
	public synchronized void closeRegisteredClients() throws IOException
	{
		for (APIEndpoint client : clients)
			client.close();
		clients.clear();
	}

	@Override
	public synchronized int countRegisteredClients()
	{
		// TODO Auto-generated method stub
		return clients.size();
	}

	public int getPort()
	{
		return port;
	}
	
	public int getMaxClients()
	{
		return maxClients;
	}
	
	public void setMaxClients(int max)
	{
		maxClients = max;
	}

	private static class APIEndpoint implements Closeable
	{
		public Socket client;
		public BufferedReader reader;
		public BufferedWriter writer;
		public InputStream inputStream;
		public OutputStream outputStream;

		public APIEndpoint(Socket client) throws IOException
		{
			this.client = client;
			inputStream = client.getInputStream();
			outputStream = client.getOutputStream();
			reader = new BufferedReader(new InputStreamReader(inputStream));
			writer = new BufferedWriter(new OutputStreamWriter(outputStream));
		}

		@Override
		public void close() throws IOException
		{
			client.close();
		}
	}
}
