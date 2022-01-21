package team.hobbyrobot.subos.logging;

import java.io.Closeable;
import java.io.File;
import java.io.IOException;
import java.io.PrintWriter;
import java.util.ArrayList;

public class Logger implements Closeable
{
	private ArrayList<PrintWriter> writers = new ArrayList<PrintWriter>();
	private ArrayList<String> localLog = new ArrayList<String>();

	public Logger()
	{
		Runtime.getRuntime().addShutdownHook(new Thread()
		{
			@Override
			public void run()
			{
				log("Shutting down...");
				try
				{
					close();
				}
				catch (IOException e)
				{
					e.printStackTrace();
				}
			}
		});
	}

	public String[] getLog()
	{
		return localLog.toArray(new String[localLog.size()]);
	}

	public synchronized void registerEndpoint(PrintWriter writer)
	{
		writers.add(writer);
	}

	public synchronized void unregisterEndpoint(PrintWriter writer)
	{
		writers.remove(writer);
	}

	public synchronized void log(String message)
	{
		localLog.add(message);

		ArrayList<PrintWriter> badWriters = new ArrayList<PrintWriter>();
		for (PrintWriter pw : writers)
		{
			try
			{
				pw.println(message);
				pw.flush();
			}
			catch (Exception ex)
			{
				badWriters.add(pw);
			}
		}

		if (badWriters.size() > 0)
		{
			for (PrintWriter pw : badWriters)
				writers.remove(pw);
			for (int i = 0; i < badWriters.size(); i++)
				log("Writer for logger threw an exception");
		}
	}

	public void logException(Exception ex)
	{
		log("Exception was thrown: " + getExceptionInfo(ex));
	}

	public static String getExceptionInfo(Exception ex)
	{
		String msg = "Error message: " + ex.getMessage() + "; Stack trace: ";

		for (StackTraceElement trace : ex.getStackTrace())
		{
			msg += "\n\t" + trace.toString();
		}

		msg += "\n; Cause: " + ex.getCause() + "; ToString: " + ex.toString();
		return msg;
	}

	@Override
	public void close() throws IOException
	{
		for (PrintWriter pw : writers)
			pw.close();
	}
}
