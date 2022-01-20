package team.hobbyrobot.subos.logging;

import java.io.IOException;
import java.io.PrintWriter;
import java.net.ServerSocket;
import java.net.Socket;
import java.net.SocketException;
import java.util.ArrayList;
import java.util.List;

public class SocketLoggerEndpointRegisterer
{
	private Logger logger;
	private int port;

	private ServerSocket serverSocket;
	private List<Socket> registeredClients = new ArrayList<Socket>();
	private Thread registeringThread;

	private boolean shouldBeRunning = false;

	public SocketLoggerEndpointRegisterer(Logger logger, int port) throws IOException
	{
		this.logger = logger;
		this.port = port;

		serverSocket = new ServerSocket(port);
	}

	public void startRegistering()
	{
		if(isRunning())
			return;
		shouldBeRunning = true;
		registeringThread = new Thread()
		{
			public void run()
			{
				while(shouldBeRunning)
				{
					try
					{
						registerClient();						
					}
					catch(IOException ex)
					{
						logger.log("IOException was thrown when registering a clinet"
							+ " in SocketLoggerEndpointRegisterer. EXCEPTION: " + Logger.getExceptionInfo(ex));
					}
				}
			}
		};
		registeringThread.start();
	}
	
	private void registerClient() throws IOException
	{
		Socket client = null;
		try
		{
			client = serverSocket.accept();
		}
		catch (SocketException ex)
		{
			logger.log("SocketException was thrown when waiting for a clinet"
				+ " in SocketLoggerEndpointRegisterer. This may indicate that the "
				+ "registering server was closed. EXCEPTION: " + Logger.getExceptionInfo(ex));
		}
		catch (IOException ex)
		{
			logger.log("IOException was thrown when waiting for a clinet"
				+ " in SocketLoggerEndpointRegisterer. EXCEPTION: " + Logger.getExceptionInfo(ex));
		}
		if(client == null)
			return;
		
		PrintWriter pw = new PrintWriter(client.getOutputStream());
		logger.registerEndpoint(pw);
		registeredClients.add(client);
	}

	public void stopRegistering() throws IOException
	{
		shouldBeRunning = false;
		serverSocket.close();
		serverSocket = null;
	}

	public int getPort()
	{
		return port;
	}
	
	public int countReisteredClients()
	{
		return registeredClients.size();
	}
	
	public void closeRegisteredClients() throws IOException
	{
		for(Socket client : registeredClients)
			client.close();
		registeredClients.clear();
	}

	public boolean isRunning()
	{
		return registeringThread != null && registeringThread.isAlive();
	}
}
