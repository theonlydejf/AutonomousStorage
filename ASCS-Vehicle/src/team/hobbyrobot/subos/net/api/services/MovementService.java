package team.hobbyrobot.subos.net.api.services;

import java.lang.reflect.Method;
import java.util.List;
import java.util.Arrays;
import java.util.Hashtable;

import lejos.hardware.motor.MotorRegulator;
import lejos.robotics.RegulatedMotor;
import lejos.robotics.chassis.Chassis;
import lejos.robotics.chassis.Wheel;
import lejos.robotics.chassis.WheeledChassis;
import lejos.robotics.chassis.WheeledChassis.Modeler;
import lejos.robotics.localization.OdometryPoseProvider;
import lejos.robotics.localization.PoseProvider;
import lejos.robotics.navigation.Move;
import lejos.robotics.navigation.MoveListener;
import lejos.robotics.navigation.MovePilot;
import lejos.robotics.navigation.MoveProvider;
import lejos.robotics.navigation.NavigationListener;
import lejos.robotics.navigation.Pose;
import lejos.robotics.navigation.Waypoint;
import team.hobbyrobot.ascsvehicle.navigation.Navigator;
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
public class MovementService implements Service, MoveListener, NavigationListener
{
	private RobotHardware hardware;
	private MovePilot pilot;
	private Logger logger;
	private PoseProvider poseProvider;
	private Navigator navigator;

	Hashtable<String, RequestInvoker> requests = null;

	public MovementService(RobotHardware hardware, Logger logger)
	{
		this.logger = logger.createSubLogger("MvService");
		this.hardware = hardware;
		pilot = null;
		poseProvider = null;
	}

	@Override
	public TDNRoot processRequest(String request, TDNRoot params)
		throws UnknownRequestException, RequestParamsException, RequestGeneralException
	{
		try
		{
			RequestInvoker requestMethod = requests.get(request);
			if (requestMethod == null)
				throw new UnknownRequestException();

			return requestMethod.invoke(params);
		}
		catch (UnknownRequestException e)
		{
			throw e;
		}
		catch (RequestParamsException e)
		{
			throw e;
		}
		catch (Exception e)
		{
			throw new RequestGeneralException(
				"Exception was thrown while performing a request: " + Logger.getExceptionInfo(e));
		}
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
		pilot.addMoveListener(this);
		
		poseProvider = new OdometryPoseProvider(pilot);
		navigator = new Navigator(pilot, poseProvider, chassis, logger);
		navigator.addNavigationListener(this);

		initRequests();
	}

	private void initRequests()
	{
		requests = new Hashtable<String, RequestInvoker>()
		{
			/**
			 * 
			 */
			private static final long serialVersionUID = 4622976006937326703L;

			{
				put("travel", new RequestInvoker()
				{
					@Override
					public TDNRoot invoke(TDNRoot params) throws RequestParamsException
					{
						TDNValue dist = params.get("distance");

						if (dist == null)
							throw new RequestParamsException("Distance doesn't exist in the current root", "distance");

						pilot.travel((float) dist.as(), true);
						return new TDNRoot();
					}
				});

				put("rotate", new RequestInvoker()
				{
					@Override
					public TDNRoot invoke(TDNRoot params) throws RequestParamsException
					{
						TDNValue ang = params.get("angle");

						if (ang == null)
							throw new RequestParamsException("Angle doesn't exist in the current root", "angle");

						pilot.rotate((float) ang.as(), true);
						return new TDNRoot();
					}
				});

				put("pose", new RequestInvoker()
				{
					@Override
					public TDNRoot invoke(TDNRoot params)
					{
						Pose pose = poseProvider.getPose();
						return new TDNRoot().insertValue("x", new TDNValue(pose.getX(), TDNParsers.FLOAT))
							.insertValue("y", new TDNValue(pose.getY(), TDNParsers.FLOAT))
							.insertValue("heading", new TDNValue(pose.getHeading(), TDNParsers.FLOAT));
					}
				});

				put("arc", new RequestInvoker()
				{
					@Override
					public TDNRoot invoke(TDNRoot params) throws RequestParamsException
					{
						TDNValue radius = params.get("radius");
						TDNValue dist = params.get("distance");
						TDNValue ang = params.get("angle");

						if (radius == null)
							throw new RequestParamsException("Radius doesn't exist in the current root", "radius");

						if (dist != null)
							pilot.travelArc((float) radius.as(), (float) dist.as(), true);
						else if (ang != null)
							pilot.arc((float) radius.as(), (float) ang.as(), true);
						else
							throw new RequestParamsException("Angle nor distance doesn't exist in the current root",
								"angle", "radius");

						return new TDNRoot();
					}
				});

				put("stop", new RequestInvoker()
				{
					@Override
					public TDNRoot invoke(TDNRoot params) throws RequestParamsException
					{
						navigator.singleStep(true);

						navigator.stop();
						return new TDNRoot();
					}
				});

				put("flt", new RequestInvoker()
				{
					@Override
					public TDNRoot invoke(TDNRoot params) throws RequestParamsException
					{
						navigator.singleStep(true);

						navigator.stop();
						hardware.LeftDriveMotor.flt();
						hardware.RightDriveMotor.flt();
						return new TDNRoot();
					}
				});

				put("goto", new RequestInvoker()
				{
					@Override
					public TDNRoot invoke(TDNRoot params) throws RequestParamsException
					{
						TDNValue x = params.get("x");
						TDNValue y = params.get("y");
						TDNValue heading = params.get("heading");
						if (x == null)
							throw new RequestParamsException("Full position isn't present in the current root", "x");
						if (y == null)
							throw new RequestParamsException("Full position isn't present in the current root", "y");
						
						navigator.clearPath();
						if (heading == null)
							navigator.goTo((float) x.as(), (float) y.as());
						else
							navigator.goTo((float) x.as(), (float) y.as(), (float) heading.as());
						return new TDNRoot();
					}
				});

				put("followPath", new RequestInvoker()
				{
					@Override
					public TDNRoot invoke(TDNRoot params) throws RequestParamsException
					{
						TDNValue waypoints = params.get("path");
						if(waypoints == null)
							throw new RequestParamsException("No path present in the current root", "path");
						
						List<TDNRoot> path = TDNValue.asList(waypoints);
						
						navigator.singleStep(false);

						navigator.clearPath();
						for(TDNRoot pose : path)
						{
							TDNValue x = pose.get("x");
							TDNValue y = pose.get("y");
							TDNValue heading = pose.get("heading");
							if (x == null)
								throw new RequestParamsException("Full position isn't present in a waypoint root", "x");
							if (y == null)
								throw new RequestParamsException("Full position isn't present in a waypoint root", "y");
							
							if (heading == null)
								navigator.addWaypoint((float) x.as(), (float) y.as());
							else
								navigator.addWaypoint((float) x.as(), (float) y.as(), (float) heading.as());
						}
						
						logger.log("NEW PATH STARTED: ");
						for(Waypoint pt : navigator.getPath())
							logger.log("\t" + pt.getPose().toString());
						navigator.followPath();
						
						return new TDNRoot();
					}
				});
				
				put("continuePath", new RequestInvoker()
				{
					@Override
					public TDNRoot invoke(TDNRoot params) throws RequestParamsException
					{
						navigator.singleStep(false);
						navigator.followPath();
						return new TDNRoot();
					}
				});
				
				put("isPathCompleted", new RequestInvoker()
				{
					@Override
					public TDNRoot invoke(TDNRoot params) throws RequestParamsException
					{
						return new TDNRoot().insertValue("pathCompleted", new TDNValue(navigator.pathCompleted(), TDNParsers.BOOLEAN));
					}
				});
				
				put("setSpeed", new RequestInvoker()
				{
					@Override
					public TDNRoot invoke(TDNRoot params) throws RequestParamsException
					{
						TDNValue speed = params.get("speed");
						if(speed == null)
							throw new RequestParamsException("No speed present in the current root", "speed");
						
						pilot.setLinearSpeed((float) speed.as());
						return new TDNRoot();
					}
				});
				
				put("setNavTravelLimit", new RequestInvoker()
				{
					@Override
					public TDNRoot invoke(TDNRoot params) throws RequestParamsException
					{
						TDNValue limit = params.get("limit");
						if(limit == null)
							throw new RequestParamsException("No limit present in the current root", "limit");
						
						navigator.setCurrentTravelLimit((float) limit.as());
						return new TDNRoot();
					}
				});
				
				put("setMaxSpeed", new RequestInvoker()
				{
					@Override
					public TDNRoot invoke(TDNRoot params) throws RequestParamsException
					{
						pilot.setLinearSpeed(pilot.getMaxLinearSpeed());
						return new TDNRoot();
					}
				});
				
				put("getSpeed", new RequestInvoker()
				{
					@Override
					public TDNRoot invoke(TDNRoot params) throws RequestParamsException
					{
						return new TDNRoot().insertValue("speed", new TDNValue((float)pilot.getLinearSpeed(), TDNParsers.FLOAT));
					}
				});
			}
		};
	}

	private abstract static class RequestInvoker
	{
		protected TDNRoot params;

		public abstract TDNRoot invoke(TDNRoot params) throws RequestParamsException;
	}

	@Override
	public void moveStarted(Move event, MoveProvider mp)
	{
		logger.log("Started move: " + event.toString() + ". Robot is at " + poseProvider.getPose().toString());
	}

	@Override
	public void moveStopped(Move event, MoveProvider mp)
	{
		logger.log("Stopped move: " + event.toString() + ". Robot is at " + poseProvider.getPose().toString());
	}

	@Override
	public void atWaypoint(Waypoint waypoint, Pose pose, int sequence)
	{
		logger.log("At waypoint " + waypoint.getPose().toString() + ", waypoints remaining + " + navigator.getPath().size() + ". Robot is at " + pose.toString());
	}

	@Override
	public void pathComplete(Waypoint waypoint, Pose pose, int sequence)
	{
		logger.log("Path completed at waypoint " + waypoint.getPose().toString() + ", waypoints remaining + " + navigator.getPath().size() + ". Robot is at " + pose.toString());
	}

	@Override
	public void pathInterrupted(Waypoint waypoint, Pose pose, int sequence)
	{
		logger.log("Path interrupted at " + pose.toString() + ", next waypoint: " + waypoint.getPose().toString());
	}

}
