package team.hobbyrobot.ascsvehicle.exe;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.Map.Entry;

import lejos.hardware.Battery;
import lejos.hardware.Button;
import lejos.hardware.Sound;
import lejos.utility.Delay;
import team.hobbyrobot.ascsvehicle.ASCSVehicleHardware;
import team.hobbyrobot.subos.SubOSController;
import team.hobbyrobot.subos.graphics.infobar.BasicInfoBar;
import team.hobbyrobot.subos.hardware.BrickHardware;
import team.hobbyrobot.subos.hardware.LEDBlinkingStyle;
import team.hobbyrobot.subos.logging.Logger;
import team.hobbyrobot.subos.logging.SocketLoggerEndpointRegisterer;
import team.hobbyrobot.subos.menu.MenuItem;
import team.hobbyrobot.subos.menu.MenuScreen;
import team.hobbyrobot.subos.menu.RobotInfoScreen;
import team.hobbyrobot.tdn.base.*;
import team.hobbyrobot.tdn.core.*;

public class ASCSVehicleRun
{
	//@formatter:off
	public static final MenuItem[] MainMenu = new MenuItem[] 
	{
		new RobotInfoScreen(ASCSVehicleHardware.class)
	};
	//@formatter:on

	/**  Inicializovany Hardware robota */
	public static ASCSVehicleHardware Hardware = new ASCSVehicleHardware(-1, -1, -1);
	/** Inicializovany InfoBar, ktery aktualne bezi */
	public static BasicInfoBar InfoBar = null;

	public static Logger logger;
	
	public static void main(String[] args) throws Exception
	{
		logger = new Logger();
		SocketLoggerEndpointRegisterer loggerRegisterer = new SocketLoggerEndpointRegisterer(logger, 1111);
		loggerRegisterer.startRegistering();
		
		SubOSController.LoadingScreenActions.add("0:team.hobbyrobot.ascsvehicle.ASCSVehicleHardware:calibrateDefaultLifter");
		
		//Inicializuj senzory v robotovi a subOS
		InfoBar = SubOSController.init(Hardware, BasicInfoBar.class, logger);

		//Dej najevo, že robot už je připraven k použití
		BrickHardware.setLEDPattern(1, LEDBlinkingStyle.NONE, 0);
		Sound.beepSequenceUp();
		
		/*int i = 0;
		while(Button.ENTER.isUp())
		{
			SubOSController.mainLogger.log("TEST - " + i++ + " - TEST, clients cnt: " + loggerRegisterer.countRegisteredClients());
			Delay.msDelay(500);
		}
		SubOSController.mainLogger.log("Repeated test ended");
		loggerRegisterer.stopRegistering();
		SubOSController.mainLogger.log("After registering stopped");*/

		logger.log("waiting for client...");
		ServerSocket server = new ServerSocket(2222);
		Sound.beep();
		Socket s = server.accept();
		logger.log("client connected!");
		Sound.twoBeeps();
		
		BufferedReader br = new BufferedReader(new InputStreamReader(s.getInputStream()));
		BufferedWriter bw = new BufferedWriter(new OutputStreamWriter(s.getOutputStream()));
		int cnt = 0;
		while(true)
		{
			TDNRoot root = TDNRoot.readFromStream(br);
			logRoot(root, logger);			
			root.put("recievedCnr", new TDNValue(++cnt, new IntegerParser()));
			root.writeToStream(bw);
		}
		//Spust menu a opakuj ho do nekonecna
		/*while (true)
		{
			Button.waitForAnyPress();
			//MenuScreen mainMenu = new MenuScreen(MainMenu);
			//mainMenu.select();
			Hardware.moveLifterTo(100);
			Hardware.fltLifter();
			Button.waitForAnyPress();
			Hardware.moveLifterTo(0);
			Hardware.fltLifter();
		}*/
	}
	
	static void logRoot(TDNRoot root, Logger logger)
	{
		logger.log("(");
		StringBuilder sb = new StringBuilder();
		for (Entry<String, TDNValue> val : root)
		{
			sb.append(val.getKey());
			sb.append(": ");
			if (val.getValue().value instanceof TDNRoot)
			{
				logRoot((TDNRoot) val.getValue().value, logger);
				continue;
			}
			if (val.getValue().value instanceof TDNArray)
			{
				logger.log(sb.toString() + "[");
				sb = new StringBuilder();
				TDNArray arr = (TDNArray) val.getValue().value;
				for (Object item : arr)
				{
					sb.append(",");
					if (arr.itemParser.typeKey().equals(new TDNRootParser().typeKey()))
						logRoot((TDNRoot) item, logger);
					else
					{
						logger.log(sb.toString() + item);
						sb = new StringBuilder();
					}					
					continue;
				}
				logger.log(sb.toString() + "]");
				sb = new StringBuilder();
				continue;
			}
			logger.log(sb.toString() + val.getValue().value);
			sb = new StringBuilder();
		}
		logger.log(sb.toString() + ")");
	}

}
