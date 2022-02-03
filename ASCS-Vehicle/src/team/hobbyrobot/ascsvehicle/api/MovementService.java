package team.hobbyrobot.ascsvehicle.api;

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
import team.hobbyrobot.tdn.core.TDNRoot;

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
		switch (request)
		{
			case "travel":
				logger.log("travelling...");
				pilot.travel((float) params.get("distance").value);
			break;

			default:
				throw new UnknownRequestException();
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
