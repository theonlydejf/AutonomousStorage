package team.hobbyrobot.ascsvehicle.api;

import java.lang.reflect.Method;
import java.util.Arrays;

import lejos.robotics.RegulatedMotor;
import lejos.robotics.chassis.Chassis;
import lejos.robotics.chassis.Wheel;
import lejos.robotics.chassis.WheeledChassis;
import lejos.robotics.chassis.WheeledChassis.Modeler;
import lejos.robotics.navigation.MovePilot;
import team.hobbyrobot.subos.SubOSController;
import team.hobbyrobot.subos.hardware.RobotHardware;
import team.hobbyrobot.subos.hardware.motor.EV3DCMediumRegulatedMotor;
import team.hobbyrobot.subos.logging.Logger;
import team.hobbyrobot.subos.net.api.Service;
import team.hobbyrobot.subos.net.api.exceptions.RequestGeneralException;
import team.hobbyrobot.subos.net.api.exceptions.RequestParamsException;
import team.hobbyrobot.subos.net.api.exceptions.UnknownRequestException;
import team.hobbyrobot.tdn.base.TDNArray;
import team.hobbyrobot.tdn.base.TDNParsers;
import team.hobbyrobot.tdn.core.TDNRoot;
import team.hobbyrobot.tdn.core.TDNValue;

// TODO opravit
public class MovementService implements Service
{
	private RobotHardware hardware;
	private MovePilot pilot;
	private Logger logger;

	public MovementService(RobotHardware hardware, Logger logger)
	{
		this.logger = logger;
		this.hardware = hardware;
		pilot = null;

	}

	@Override
	public TDNRoot processRequest(String request, TDNRoot params)
		throws UnknownRequestException, RequestParamsException, RequestGeneralException
	{
		/*Method method;
		try
		{
			TDNValue paramTypesTDN = params.get("paramTypes");
			if(paramTypesTDN == null || !paramTypesTDN.parser().typeKey().equals(TDNParsers.ARRAY.typeKey()))
				throw new RequestParamsException("correct form of paramTypes doesn't exist", "paramTypes");
			
			TDNArray paramTypesRaw = params.get("paramTypes").as();
			if(!paramTypesRaw.itemParser.typeKey().equals(TDNParsers.STRING.typeKey()))
				throw new RequestParamsException("correct form of paramTypes doesn't exist", "paramTypes");
			
			String[] paramTypesStr = Arrays.copyOf(paramTypesRaw.value, paramTypesRaw.value.length, String[].class);
			Class<?>[] paramTypes = new Class<?>[paramTypesStr.length];
			for(int i = 0; i < paramTypes.length; i++)
				paramTypes[i] = Class.forName(paramTypesStr[i]);
			
			method = MovePilot.class.getMethod(request, paramTypes);
		}
		catch (SecurityException e)
		{
			throw new RequestGeneralException(Logger.getExceptionInfo(e));
		}
		catch (NoSuchMethodException e)
		{
			throw new UnknownRequestException();
		}
		catch (ClassNotFoundException e)
		{
			throw new RequestParamsException("Unknown class type: " + Logger.getExceptionInfo(e), "paramTypes");
		}
		
		method.invoke(pilot, )*/
		
		try
		{
			switch(request)
			{
				case "travel":
					pilot.travel((float)params.get("distance").as(), true);
					break;
				case "rotate":
					pilot.rotate((float)params.get("angle").as(), true);
					break;
				default:
					throw new UnknownRequestException();
			}			
		}
		catch(Exception e)
		{
			throw new RequestGeneralException(Logger.getExceptionInfo(e));
		}
		
		return new TDNRoot();
	}

	@Override
	public void init()
	{
		Wheel lWheel = WheeledChassis
			.modelWheel((EV3DCMediumRegulatedMotor) hardware.LeftDriveMotor, hardware.WheelRadius)
			.offset(hardware.WheelDistance / 2f).invert(true);
		Wheel rWheel = WheeledChassis
			.modelWheel((EV3DCMediumRegulatedMotor) hardware.RightDriveMotor, hardware.WheelRadius)
			.offset(-hardware.WheelDistance / 2f);
		Chassis chassis = new WheeledChassis(new Wheel[] { lWheel, rWheel }, WheeledChassis.TYPE_DIFFERENTIAL);
		pilot = new MovePilot(chassis);
		pilot.setAngularAcceleration(400);
		pilot.setLinearAcceleration(400);

	}

}
