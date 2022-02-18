package team.hobbyrobot.subos.net;

import java.io.BufferedReader;
import java.io.Closeable;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.io.PrintWriter;
import java.net.ServerSocket;
import java.net.Socket;

import lejos.hardware.Sound;
import team.hobbyrobot.subos.logging.Logger;
import team.hobbyrobot.subos.navigation.PID;

public class PIDTuner extends PID
{
	private Tuner _tuner;
	private int _port;
	
	public PIDTuner(double p, double i, double d, double f, int port) throws IOException
	{
		super(p, i, d, f);
		_port = port;
		startTuner();
	}

	public void startTuner() throws IOException
	{
		if(_tuner != null && isTunerRunning())
			stopTuner();
		
		_tuner = new Tuner(_port);
		_tuner.setDaemon(true);
		_tuner.start();
	}
	
	public boolean isTunerRunning()
	{
		return _tuner.isAlive();
	}
	
	public boolean isTunerActive()
	{
		return _tuner.active;
	}
	
	public void stopTuner() throws IOException
	{
		_tuner.stopTuner();
	}
	
	protected class Tuner extends Thread implements Closeable
	{
		public boolean active;
		
		ServerSocket _server;
		
		public Tuner(int port) throws IOException
		{
			 _server = new ServerSocket(port);
		}
		
		@Override
		public void run()
		{
			try(Socket client = _server.accept())
			{
				BufferedReader br = new BufferedReader(new InputStreamReader(client.getInputStream()));
				PrintWriter pr = new PrintWriter(new OutputStreamWriter(client.getOutputStream()));
				pr.println(PIDTuner.this.toString());
				pr.flush();
				active = true;
				while(active)
				{
					String cmd = br.readLine();
					try
					{
						String[] parts = cmd.split("\\s");
						switch(parts[0].toLowerCase())
						{
							case "p":
								PIDTuner.this.setP(Double.parseDouble(parts[1]));
								break;
								
							case "i":
								PIDTuner.this.setI(Double.parseDouble(parts[1]));
								break;
								
							case "d":
								PIDTuner.this.setD(Double.parseDouble(parts[1]));
								break;
								
							case "f":
								PIDTuner.this.setF(Double.parseDouble(parts[1]));
								break;
								
							case "maxi":
								PIDTuner.this.setMaxIOutput(Double.parseDouble(parts[1]));
								break;
								
							case "limits":
								if(parts.length > 2)
									PIDTuner.this.setOutputLimits(Double.parseDouble(parts[1]), Double.parseDouble(parts[2]));
								else
									PIDTuner.this.setOutputLimits(Double.parseDouble(parts[1]));
								break;
								
							case "dir":
								PIDTuner.this.setDirection(parts[1].toLowerCase().contains("t"));
								break;
								
							case "outramprate":
								PIDTuner.this.setOutputRampRate(Double.parseDouble(parts[1]));
								break;
								
							case "outfilter:":
								PIDTuner.this.setOutputFilter(Double.parseDouble(parts[1]));
								break;
								
							default:
								pr.println("Unknown command: " + parts[0]);
								break;
						}
						pr.println(PIDTuner.this.toString());
						pr.flush();
						Sound.beep();
					}
					catch (Exception e)
					{
						pr.println(Logger.getExceptionInfo(e));
					}
				}
			}
			catch (IOException e)
			{
				e.printStackTrace();
			}
			active = false;
		}

		public void stopTuner() throws IOException
		{
			active = false;
			close();
		}

		@Override
		public void close() throws IOException
		{
			_server.close();
		}
	}
}
