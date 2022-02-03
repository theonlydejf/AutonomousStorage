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
import team.hobbyrobot.ascsvehicle.api.MovementService;
import team.hobbyrobot.ascsvehicle.api.TestService;
import team.hobbyrobot.subos.SubOSController;
import team.hobbyrobot.subos.errorhandling.ErrorLogging;
import team.hobbyrobot.subos.graphics.GraphicsController;
import team.hobbyrobot.subos.graphics.infobar.BasicInfoBar;
import team.hobbyrobot.subos.hardware.BrickHardware;
import team.hobbyrobot.subos.hardware.LEDBlinkingStyle;
import team.hobbyrobot.subos.hardware.motor.EV3DCMediumRegulatedMotor;
import team.hobbyrobot.subos.logging.Logger;
import team.hobbyrobot.subos.logging.SocketLoggerEndpointRegisterer;
import team.hobbyrobot.subos.menu.MenuItem;
import team.hobbyrobot.subos.menu.MenuScreen;
import team.hobbyrobot.subos.menu.RobotInfoScreen;
import team.hobbyrobot.subos.net.api.TDNAPIServer;
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
		// Starts main logger
		logger = new Logger();
		SocketLoggerEndpointRegisterer loggerRegisterer = new SocketLoggerEndpointRegisterer(logger, 1111);
		loggerRegisterer.startRegisteringClients();
		
		SubOSController.LoadingScreenActions.add("0:team.hobbyrobot.ascsvehicle.ASCSVehicleHardware:calibrateDefaultLifter");
		
		// Starts subOS
		InfoBar = SubOSController.init(Hardware, BasicInfoBar.class, logger, "error_log.txt");

		//Dej najevo, že robot už je připraven k použití
		BrickHardware.setLEDPattern(1, LEDBlinkingStyle.NONE, 0);
		Sound.beepSequenceUp();

		TDNAPIServer api = new TDNAPIServer(2222, logger, SubOSController.errorLogger);
		api.registerService("TestService", new TestService());
		api.registerService("MovementService", new MovementService(Hardware, logger));
		
		Thread t = new Thread(api);
		t.setDaemon(true);
		t.start();
		
		logger.log("started registering...");
		api.startRegisteringClients();
		
		//Spust menu a opakuj ho do nekonecna
		GraphicsLCD g = GraphicsController.getNewDefaultMainGraphics();
		g.clear();

		GraphicsController.refreshScreen();
		
		logger.log("Creating pilot..");
		Wheel lWheel = WheeledChassis.modelWheel((EV3DCMediumRegulatedMotor)Hardware.LeftDriveMotor, 4.95f).offset(5.5f);
		Wheel rWheel = WheeledChassis.modelWheel((EV3DCMediumRegulatedMotor)Hardware.RightDriveMotor, 4.95f).offset(5.5f).invert(true);
		Chassis chassis = new WheeledChassis(new Wheel[] { lWheel, rWheel }, WheeledChassis.TYPE_DIFFERENTIAL);
		MovePilot pilot = new MovePilot(chassis);
		logger.log("pilot done!");
		
		while (true)
		{
			pilot.travel(10);
			pilot.rotate(90);
			Sound.beep();
			Button.waitForAnyPress();
		}
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
