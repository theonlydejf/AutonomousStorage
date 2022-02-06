package team.hobbyrobot.ascsvehicle.exe;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.Map.Entry;

import lejos.hardware.Battery;
import lejos.hardware.Button;
import lejos.hardware.Sound;
import lejos.hardware.lcd.GraphicsLCD;
import lejos.robotics.chassis.Chassis;
import lejos.robotics.chassis.Wheel;
import lejos.robotics.chassis.WheeledChassis;
import lejos.robotics.navigation.MovePilot;
import lejos.utility.Delay;
import team.hobbyrobot.ascsvehicle.ASCSVehicleHardware;
import team.hobbyrobot.ascsvehicle.api.services.TestService;
import team.hobbyrobot.subos.LoadingScreen;
import team.hobbyrobot.subos.SubOSController;
import team.hobbyrobot.subos.errorhandling.ErrorLogging;
import team.hobbyrobot.subos.graphics.GraphicsController;
import team.hobbyrobot.subos.graphics.infobar.BasicInfoBar;
import team.hobbyrobot.subos.hardware.BrickHardware;
import team.hobbyrobot.subos.hardware.LEDBlinkingStyle;
import team.hobbyrobot.subos.hardware.motor.EV3DCMediumRegulatedMotor;
import team.hobbyrobot.subos.logging.Logger;
import team.hobbyrobot.subos.logging.SocketLoggerEndpointRegisterer;
import team.hobbyrobot.subos.logging.VerbosityLogger;
import team.hobbyrobot.subos.menu.MenuItem;
import team.hobbyrobot.subos.menu.MenuScreen;
import team.hobbyrobot.subos.menu.RobotInfoScreen;
import team.hobbyrobot.subos.net.api.APIStaticFactory;
import team.hobbyrobot.subos.net.api.TDNAPIServer;
import team.hobbyrobot.subos.net.api.services.MovementService;
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
	public static ASCSVehicleHardware Hardware = new ASCSVehicleHardware(130, 49.5f);
	/** Inicializovany InfoBar, ktery aktualne bezi */
	public static BasicInfoBar InfoBar = null;

	public static Logger logger;

	public static TDNAPIServer api = null;
	
	public static void main(String[] args) throws Exception
	{
		// Starts main logger
		logger = new Logger();
		SocketLoggerEndpointRegisterer loggerRegisterer = new SocketLoggerEndpointRegisterer(logger, 1111);
		loggerRegisterer.startRegisteringClients();

		//SubOSController.LoadingScreenActions.add("0:team.hobbyrobot.ascsvehicle.ASCSVehicleHardware:calibrateDefaultLifter");

		// Starts subOS
		InfoBar = SubOSController.init(Hardware, BasicInfoBar.class, logger, "error_log.txt");
		initVehicle();

		//Dej najevo, že robot už je připraven k použití
		BrickHardware.setLEDPattern(1, LEDBlinkingStyle.NONE, 0);
		Sound.beepSequenceUp();

		api.setVerbosity(VerbosityLogger.OVERVIEW);
		
		//Spust menu a opakuj ho do nekonecna
		GraphicsLCD g = GraphicsController.getNewDefaultMainGraphics();
		g.clear();

		GraphicsController.refreshScreen();

		while (true)
		{
			Button.waitForAnyPress();
			Hardware.moveLifterTo(100);
			Button.waitForAnyPress();
			Hardware.moveLifterTo(0);
		}
	}

	private static void initVehicle()
	{
		// Calibrate robot
		new LoadingScreen("Calibration",
			new String[] { "0:team.hobbyrobot.ascsvehicle.ASCSVehicleHardware:calibrateDefaultLifter" }).start();
		
		APIStaticFactory.setInfoLogger(logger);
		APIStaticFactory.setVerbosity(VerbosityLogger.DEBUGGING);
		
		APIStaticFactory.reset();
		APIStaticFactory.setAPILogger(logger);
		APIStaticFactory.setPort(2222);
		APIStaticFactory.settings.setStartServer(true);
		APIStaticFactory.settings.setStartRegisteringClients(true);
		APIStaticFactory.queueService("TestService", new TestService());
		APIStaticFactory.queueService("MovementService", new MovementService(Hardware, logger));
		
		new LoadingScreen("Starting API", 
			new String[] { "0:team.hobbyrobot.subos.net.api.APIStaticFactory:createAPI" }).start();
		api = APIStaticFactory.getLastAPIServer();
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
