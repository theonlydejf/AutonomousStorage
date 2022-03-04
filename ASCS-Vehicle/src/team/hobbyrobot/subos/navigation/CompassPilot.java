package team.hobbyrobot.subos.navigation;

import java.io.IOException;
import java.util.HashMap;
import java.util.HashSet;
import java.util.LinkedList;
import java.util.List;

import lejos.robotics.navigation.Move;
import lejos.robotics.navigation.Move.MoveType;
import lejos.robotics.navigation.MoveListener;
import lejos.robotics.navigation.RotateMoveController;
import lejos.utility.Stopwatch;
import team.hobbyrobot.subos.SubOSController;
import team.hobbyrobot.subos.hardware.GyroRobotHardware;
import team.hobbyrobot.subos.net.PIDTuner;

public class CompassPilot extends MoveHandler implements RotateMoveController, LimitablePilot
{
	private int _angularSpeed; // %
	private int _linearSpeed; // %
	private int _linearMinSpeed; // %
	private int _angularMinSpeed; // %
	private double _linearAccel; // %/sec
	private double _angularAccel; // %/sec
	
	private float _expectedHeading;

	private TravelProcessor _travelProcessor;
	private RotateProcessor _rotateProcessor;
	
	public CompassPilot(GyroRobotHardware hardware)
	{
		super(hardware);

		_rotateProcessor = new RotateProcessor();
		_travelProcessor = new TravelProcessor();
		registerProcessor(MoveType.ROTATE, _rotateProcessor);
		registerProcessor(MoveType.TRAVEL, _travelProcessor);
		
		_angularSpeed = 100;
		_linearSpeed = 100;
		
		_linearAccel = 100;
		_angularAccel = 150;

		_linearMinSpeed = 30;
		_angularMinSpeed = 25;
		
		_expectedHeading = hardware.getAngle();
	}

	private void waitForMoveFinish()
	{
		while (!moveHandler.isMoving())
			Thread.yield();
		while (moveHandler.isMoving())
			Thread.yield();
	}

	@Override
	public void forward()
	{
		travel(Double.POSITIVE_INFINITY, true);
	}

	@Override
	public void backward()
	{
		travel(Double.NEGATIVE_INFINITY, true);
	}

	@Override
	public void stop()
	{
		moveHandler.stopMove();
	}

	@Override
	public boolean isMoving()
	{
		return moveHandler.isMoving();
	}

	@Override
	public void travel(double distance)
	{
		travel(distance, false);
	}

	@Override
	public void travel(double distance, boolean immediateReturn)
	{
		
		
		moveHandler.startNewMove(
			new Move(MoveType.TRAVEL, (float) distance, 0, _linearSpeed, _angularSpeed, moveHandler.isMoving()));
		if (immediateReturn)
			return;

		waitForMoveFinish();
	}

	@Override
	public void setLinearSpeed(double speed)
	{
		_linearSpeed = (int) speed;
	}

	@Override
	public double getLinearSpeed()
	{
		return _linearSpeed;
	}

	@Override
	public double getMaxLinearSpeed()
	{
		return 100;
	}

	@Override
	public void setLinearAcceleration(double acceleration)
	{
		_linearAccel = acceleration;
	}

	@Override
	public double getLinearAcceleration()
	{
		return _linearAccel;
	}

	@Override
	public void rotate(double angle)
	{
		rotate(angle, false);
	}

	@Override
	public void rotate(double angle, boolean immediateReturn)
	{
		moveHandler.startNewMove(
			new Move(MoveType.ROTATE, 0, (float) angle, _linearSpeed, _angularSpeed, moveHandler.isMoving()));
		if (immediateReturn)
			return;

		waitForMoveFinish();
	}

	@Override
	public void setAngularSpeed(double speed)
	{
		_angularSpeed = (int) speed;
	}

	@Override
	public double getAngularSpeed()
	{
		return _angularSpeed;
	}

	@Override
	public double getMaxAngularSpeed()
	{
		return 100;
	}

	@Override
	public void setAngularAcceleration(double acceleration)
	{
		_angularAccel = acceleration;
	}

	@Override
	public double getAngularAcceleration()
	{
		return _angularAccel;
	}

	@Override
	public void rotateRight()
	{
		rotate(Double.NEGATIVE_INFINITY, true);
	}

	@Override
	public void rotateLeft()
	{
		rotate(Double.POSITIVE_INFINITY, true);
	}

	@Override
	public void setTravelLimit(double limit)
	{
		moveHandler.setTravelLimit(limit);
	}

	@Override
	public double getTravelLimit()
	{
		return moveHandler.getTravelLimit();
	}

	@Override
	public void setRotateLimit(double limit)
	{
		moveHandler.setRotateLimit(limit);
	}

	@Override
	public double getRotateLimit(double limit)
	{
		return moveHandler.getRotateLimit();
	}

	public int getLinearMinSpeed()
	{
		return _linearMinSpeed;
	}

	public void setLinearMinSpeed(int minSpeed)
	{
		_linearMinSpeed = minSpeed;
	}
	
	public int getAngulatMinSpeed()
	{
		return _angularMinSpeed;
	}

	public void setAngularMinSpeed(int minSpeed)
	{
		_angularMinSpeed = minSpeed;
	}
	
	public void setExpectedHeading(float heading)
	{
		_expectedHeading = heading;
	}
	
	private class RotateProcessor implements MoveProcessor
	{
		private PID _pid;
		private float _angleRotatedAtMoveStart = 0;
		private Accelerator _accelerator;

		public RotateProcessor()
		{
			_pid = null;
			try
			{
				_pid = new PIDTuner(.8, .0001, 0, 0, 1234);
			}
			catch (IOException e)
			{
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}

		@Override
		public boolean step(Move targetMove)
		{
			// If the robot should rotate without limit -> skip regulation
			if(Double.isInfinite(targetMove.getAngleTurned()))
			{
				controlMotors((int)(targetMove.getRotateSpeed() * Math.signum(targetMove.getAngleTurned())), 100);
				return false;
			}
			
			// Angle travelled since the move has started
			float travelledAng = hardware.getAngle() - _angleRotatedAtMoveStart;

			// Limit the rotate rate pid to the maximum rotate speed
			_pid.setOutputLimits(targetMove.getRotateSpeed());

			// Calculate speed, based on how close the robot is to the target angle,
			// The closer it is - the slower the robot moves
			float decelSpeed = (float) _pid.getOutput(travelledAng, targetMove.getAngleTurned());
			float absDecelSpeed = Math.abs(decelSpeed);
			
			// If the decel speed is lower then angular min speed -> bound it to the min angular speed
			if(absDecelSpeed < _angularMinSpeed)
			{
				decelSpeed = _angularMinSpeed * Math.signum(decelSpeed);
				absDecelSpeed = _angularMinSpeed;
			}
			
			// Calculate the accelerating speed
			float currSpeed = (float)_accelerator.getCurrentSpeed(_linearAccel);
			
			// If robot should go slower, then the accelerator suggests, use the decelSpeed
			if(absDecelSpeed < currSpeed)
				currSpeed = absDecelSpeed;
			
			// Update motors
			controlMotors((int)currSpeed, 100 * Math.signum(decelSpeed));
			
			// True, if the move has completed
			boolean rotateCompleted = travelledAng == targetMove.getAngleTurned();
			
			// If the move has completed -> set the heading, at which the robot is expected to be
			if(rotateCompleted)
				_expectedHeading = targetMove.getAngleTurned();
			
			return rotateCompleted;
		}

		@Override
		public void reset()
		{
			resetSteeringControlLoop();

			_pid.reset();
			_angleRotatedAtMoveStart = hardware.getAngle();
			_accelerator = new Accelerator(_angularMinSpeed);
		}

	}

	private class TravelProcessor implements MoveProcessor
	{
		public static final float DECEL_CONSTANT = .25f;
		public static final int PID_CONTROL_PERIOD = 10; //ms
		
		public float targetAngle = 0;
		
		private PID _pid;
		private float _distanceTravelledAtMoveStart = 0;
		private Accelerator _accelerator;
		private Stopwatch _pidSw;
		private double _currPIDRate;
		
		public TravelProcessor()
		{
			_pid = null;
			try
			{
				_pid = new PIDTuner(0, 0, 0, 0, 1235);
				_pid.setOutputLimits(100);
				_pid.setMaxIOutput(50);
				_pid.verbal = true;
			}
			catch (IOException e)
			{
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
			_pidSw = new Stopwatch();
		}
		
		@Override
		public boolean step(Move targetMove)
		{
			float travelTarget = targetMove.getDistanceTraveled();
			float absTravelTarget = Math.abs(travelTarget);
			
			float travelled = hardware.getDrivenDist() - _distanceTravelledAtMoveStart;
			float absTravelled = Math.abs(travelled);
			
			float distanceRemaining = absTravelTarget - absTravelled;
			
			float decelSpeed = distanceRemaining * DECEL_CONSTANT + _linearMinSpeed;
			if(decelSpeed > targetMove.getTravelSpeed())
				decelSpeed = targetMove.getTravelSpeed();
			else if(decelSpeed < _linearMinSpeed)
				decelSpeed = _linearMinSpeed;
			
			float currSpeed = (float)_accelerator.getCurrentSpeed(_linearAccel);
			if(decelSpeed < currSpeed)
				currSpeed = decelSpeed;
			
			float currAng = hardware.getAngle();
			if(_pidSw.elapsed() >= MoveHandler.STEERING_CONTROL_LOOP_PERIOD)
			{
				_pidSw.reset();
				_currPIDRate = _pid.getOutput(currAng, targetAngle);
				
				controlMotors((int)currSpeed, _currPIDRate);
				/*int lPower = (int)(currSpeed * Math.signum(travelTarget));
				
				int rPower = (int)(currSpeed * Math.signum(travelTarget));
				
				// Calculate regulated motor powers
				if (_currPIDRate > 0)
					lPower -= (int) (Math.abs(_currPIDRate) * Math.signum(lPower));
				else
					rPower -= (int) (Math.abs(_currPIDRate) * Math.signum(rPower));

				// Limit regulated motors to only slow down (never change direction of the motor by regulating it)
				//if (lPower * Math.signum(travelTarget) < 0)
				//	lPower = 0;
				//if (rPower * Math.signum(travelTarget) < 0)
				//	rPower = 0;

				hardware.setDrivePowers(lPower, rPower);
				hardware.startDriveMotors(true);*/
			}
			
			boolean travelCompleted = distanceRemaining <= 0;
			return travelCompleted;
		}

		@Override
		public void reset()
		{
			resetSteeringControlLoop();
			
			_distanceTravelledAtMoveStart = hardware.getDrivenDist();
			_accelerator = new Accelerator(_linearMinSpeed);
			targetAngle = _expectedHeading;
			_pidSw.reset();
			_pid.reset();
			_currPIDRate = 0;
		}

	}
}
