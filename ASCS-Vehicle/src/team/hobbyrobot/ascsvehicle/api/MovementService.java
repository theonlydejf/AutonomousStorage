package team.hobbyrobot.ascsvehicle.api;

import lejos.robotics.RegulatedMotor;
import lejos.robotics.chassis.Chassis;
import lejos.robotics.chassis.Wheel;
import lejos.robotics.chassis.WheeledChassis;
import lejos.robotics.chassis.WheeledChassis.Modeler;
import lejos.robotics.navigation.MovePilot;
import team.hobbyrobot.subos.SubOSController;
import team.hobbyrobot.subos.hardware.RobotHardware;
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
		Modeler wheel1 = WheeledChassis.modelWheel((RegulatedMotor) hardware.LeftDriveMotor, 49.5).offset(-11.2 / 2);
		wheel1.invert(true);
		Modeler wheel2 = WheeledChassis.modelWheel((RegulatedMotor) hardware.RightDriveMotor, 49.5).offset(11.2 / 2);
		Chassis chassis = new WheeledChassis(new Wheel[] { wheel1, wheel2 }, WheeledChassis.TYPE_DIFFERENTIAL);
		pilot = new MovePilot(chassis);

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

}
