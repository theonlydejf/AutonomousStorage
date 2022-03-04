package team.hobbyrobot.subos.navigation;

import java.io.IOException;
import java.text.DecimalFormat;
import java.util.Hashtable;
import java.util.LinkedList;
import java.util.List;

import lejos.robotics.navigation.*;
import lejos.robotics.navigation.Move.MoveType;
import lejos.utility.Stopwatch;
import team.hobbyrobot.subos.SubOSController;
import team.hobbyrobot.subos.hardware.GyroRobotHardware;
import team.hobbyrobot.subos.net.PIDTuner;

public class MoveHandler implements MoveProvider
{
	/**
	 * How often is motor controlled when regulated steering. Higher value means more precise regulation,
	 * but smaller responsibility.
	 */
	public final static int STEERING_CONTROL_LOOP_PERIOD = 50; //ms

	/** All listeneres, that have signed up for listening to this MoveProvider */
	protected List<MoveListener> _listeners;

	/** Instance of {@link MoveHandler.Handler}, that is used to handle moves for this MoveHandler */
	protected Handler moveHandler;
	/** Instance of {@link GyroRobotHardware}, that describes the robot */
	protected GyroRobotHardware hardware;

	/** Tachometer state of left motor, after last run of steering control loop */
	private int _steering_lTacho = 0;
	/** Tachometer state of right motor, after last run of steering control loop */
	private int _steering_rTacho = 0;
	/** Steering the robot is expected to travel at, since last run of steering control loop */
	private double _steering_expectedRatio = 0;
	
	private boolean _steering_expectedLeft = false;

	/**
	 * True, when the steering control loop is running for the first time, since the
	 * {@link #resetSteeringControlLoop} method was called
	 */
	private boolean _steering_firstRun = true;
	/** Stopwatch used to limit the steering control loop */
	private Stopwatch _steeringControlLoopSw = new Stopwatch();
	/** Regulator used regulate steering */
	private PID _steeringPID;

	/**
	 * Creates instance of {@link MoveHandler} and starts the underlying {@link MoveHandler.Handler} that will
	 * handle move requests
	 * 
	 * @param hardware Instance of {@link GyroRobotHardware}, that describes the robot
	 */
	public MoveHandler(GyroRobotHardware hardware)
	{
		this.hardware = hardware;
		_listeners = new LinkedList<MoveListener>();

		moveHandler = new Handler();
		moveHandler.setDaemon(true);
		moveHandler.start();

		try
		{
			_steeringPID = new PIDTuner(70, 8, 0, 0, 1236);
			_steeringPID.verbal = true;
		}
		catch (IOException e)
		{
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
		_steeringPID.setMaxIOutput(40);
	}

	@Override
	public Move getMovement()
	{
		if (moveHandler.isMoving())
			return moveHandler.getDisplacement();
		else
			return new Move(Move.MoveType.STOP, 0, 0, false);
	}

	@Override
	public void addMoveListener(MoveListener listener)
	{
		_listeners.add(listener);
	}

	/**
	 * Registers a new move processor, that is capable of processing a certain move type, to the underlying
	 * move handler
	 * 
	 * @param moveType  {@link MoveType}, that the processor is capable of handling
	 * @param processor Instance of {@link MoveProcessor}, that can process the desired move type
	 */
	protected void registerProcessor(MoveType moveType, MoveProcessor processor)
	{
		synchronized (moveHandler.processors)
		{
			moveHandler.processors.put(moveType, processor);
		}
	}

	/** Prepares steering control loop for a new move */
	public void resetSteeringControlLoop()
	{
		_steering_lTacho = hardware.getLeftTacho();
		_steering_rTacho = hardware.getRightTacho();
		_steeringControlLoopSw.reset();

		// Resets regulator and leaves the integral part intact
		_steeringPID.reset(false);

		_steering_firstRun = true;
	}

	/**
	 * Controls robot's drive motors, given power and steering. Steering is a number between -100 and 100,
	 * where -100 means rotate to the right, 0 move straight and 100 means rotate to the left. Any number
	 * in between makes the robot go in arc.<br>
	 * This method is limited to be called only once every {@link #STEERING_CONTROL_LOOP_PERIOD}
	 * milliseconds. If the method is called, before the required time has passed, it returns false without
	 * doing anything.
	 * 
	 * @param power    The desired power by which the robot should move (-100(backward) to 100(forward))
	 * @param steering The steering by which the robot should move (-100(right) to 100(left))
	 * @return True, if motors were updated
	 */
	public boolean controlMotors(int power, double steering)
	{
		// We need some time for tacho to acumulate -> if not enaugh time has passed yet, return without 
		// doing anything
		if (_steeringControlLoopSw.elapsed() < STEERING_CONTROL_LOOP_PERIOD)
			return false;
		_steeringControlLoopSw.reset();

		// Limit steering
		if (steering < -100)
			steering = -100;
		else if (steering > 100)
			steering = 100;

		// Limit power
		if (power < -100)
			power = -100;
		else if (power > 100)
			power = 100;

		// If robot only should use one motor -> skip regulation and just break the motor
		if (Math.abs(Math.round(steering)) == 50)
		{
			if (steering > 0)
			{
				hardware.setRightDrivePower(power);
				hardware.startRightDriveMotor(true);
				hardware.stopLeftDriveMotor(true);
				return true;
			}
			hardware.setLeftDrivePower(power);
			hardware.startLeftDriveMotor(true);
			hardware.stopRightDriveMotor(true);
			return true;
		}

		// True, if according to steering, robot is steering to the left
		boolean steeringLeft = steering > 0;

		// Calculate real motor ratio since last control
		int deltaLTacho = hardware.getLeftTacho() - _steering_lTacho;
		int deltaRTacho = hardware.getRightTacho() - _steering_rTacho;

		int innerTacho = steeringLeft ? deltaLTacho : deltaRTacho;
		int outerTacho = steeringLeft ? deltaRTacho : deltaLTacho;

		// If outer motor hasn't moved since last regulation, make it non zero value 
		// (so a division by zero won't happen)
		if (outerTacho == 0)
			outerTacho = 1;

		double realRatio = innerTacho / (double) outerTacho;

		// True, if too slow to regulate
		boolean tooSlow = (Math.abs(deltaLTacho) + Math.abs(deltaRTacho)) / 2 < 7;

		//TODO add regulation..
		// Calculate correction value, if the robot is moving too slowly to regulate, get 
		// only integral part of the regulation (makes the pid error 0)
		double pidRate = _steeringPID.getOutput(tooSlow ? _steering_expectedRatio : realRatio, _steering_expectedRatio);
		//SubOSController.mainLogger.log("steering integral: " + _steeringPID.Integral);
		// TODO: remove
		// log debug data to the main logger
		/*
		 * DecimalFormat f = new DecimalFormat("#.##");
		 * SubOSController.mainLogger.log("expected: " + f.format(_steering_expectedRatio) + "\treal: "
		 * + f.format(realRatio) + "\tpid: " + f.format(pidRate) + "\tinner: " + f.format(innerTacho) +
		 * "\touter: "
		 * + f.format(outerTacho) + "\tpower: " + f.format(power) + "\ttooSlow: " + tooSlow);
		 */

		if (_steering_firstRun)
		{
			pidRate = 0;
			_steering_firstRun = false;
		}

		// Calculate current powers
		double steeringMultiplier = (Math.abs(steering) - 50) / -50;

		int baseLPower = (int) (power * (steeringLeft ? steeringMultiplier : 1));
		int baseRPower = (int) (power * (steeringLeft ? 1 : steeringMultiplier));
		int lPower = baseLPower;
		int rPower = baseRPower;

		if(_steering_expectedRatio != steeringMultiplier && steeringLeft != _steering_expectedLeft)
		{
			pidRate = 0;
			_steeringPID.reset();
		}
		
		// Calculate regulated motor powers
		if (pidRate > 0 ^ (steeringLeft ^ Math.signum(steeringMultiplier) != 1))
			lPower -= (int) (Math.abs(pidRate) * Math.signum(lPower));
		else
			rPower -= (int) (Math.abs(pidRate) * Math.signum(rPower));

		// Limit regulated motors to only slow down (never change direction of the motor by regulating it)
		if (lPower * Math.signum(baseLPower) < 0)
			lPower = 0;
		if (rPower * Math.signum(baseRPower) < 0)
			rPower = 0;

		hardware.setDrivePowers(lPower, rPower);
		hardware.startDriveMotors(true);

		_steering_lTacho = hardware.getLeftTacho();
		_steering_rTacho = hardware.getRightTacho();
		_steering_expectedRatio = steeringMultiplier;
		_steering_expectedLeft = steeringLeft;
		
		return true;
	}

	/**
	 * Class, that is used to handle a perform move requests
	 * 
	 * @author David Krcmar
	 */
	public class Handler extends Thread
	{
		//TODO: rename to defaultProcessors
		/** Map of MoveType to a processor class, that can perform that move */
		Hashtable<MoveType, MoveProcessor> processors;

		/** Move, that is requested to be handled */
		private Move _requestedMove;
		/** Move, that is currently being handled by the controller */
		private Move _currMove;
		/**
		 * Real move, that is currently being handled by the controller (may be limitted by travel or angle
		 * limits)
		 */
		private Move _currTargetMove;
		/** True when move is being handled (if robot is moving) */
		private boolean _moveActive = false;
		/** True when a new move is requested */
		private boolean _moveRequested = false;

		/** True, when any limit has changed and those changes weren't handled by the controller yet */
		private boolean _limitChanged = false;
		/**
		 * Maximum angle, the robot can rotate during the current move (is reseted to infinity after move has
		 * been completed)
		 */
		private double _angLimit = Double.POSITIVE_INFINITY;
		/**
		 * Maximum distance, the robot can travel during the current move (is reseted to infinity after move
		 * has
		 * been completed)
		 */
		private double _distLimit = Double.POSITIVE_INFINITY;

		/** True, when controller is able to handle move requests */
		private boolean _controllerActive = false;

		/** Distance robot has already travelled, when a new move has started */
		private float _distanceTravelledAtMoveStart = 0;
		/** Angle robot has already rotated, when a new move has started */
		private float _angleRotatedAtMoveStart = 0;

		public Handler()
		{
			processors = new Hashtable<>();
		}

		/** Stops the robot and stops listening for move requests */
		public void deactivate()
		{
			stopMove();
			_controllerActive = false;
		}

		/** True, when controller is listening for new move requests */
		public boolean isActive()
		{
			return _controllerActive;
		}

		/**
		 * Sets the maximum distance, the robot can travel during the current move (is reseted to infinity
		 * after move has been completed)
		 * 
		 * @param limit Maximum distance, the robot can travel
		 */
		public void setTravelLimit(double limit)
		{
			_distLimit = Math.abs(limit);
			_limitChanged = true;
		}

		/**
		 * Sets the maximum angle, the robot can rotate during the current move (is reseted to infinity after
		 * move has been completed)
		 * 
		 * @param limit Maximum angle, the robot can rotate
		 */
		public void setRotateLimit(double limit)
		{
			_angLimit = Math.abs(limit);
			_limitChanged = true;
		}

		/**
		 * Gets the maximum distance, the robot can travel during the current move
		 * 
		 * @return The travel limit
		 */
		public double getTravelLimit()
		{
			return _angLimit;
		}

		/**
		 * Gets the maximum angle, the robot can rotate during the current move
		 * 
		 * @return The rotate limit
		 */
		public double getRotateLimit()
		{
			return _distLimit;
		}

		//TODO: add possibility to to specify a specific processor to use
		/** Requests a new move and waits, until the current move (if any) is canceled */
		public void startNewMove(Move move)
		{
			_requestedMove = move;
			_moveRequested = true;
			// Waits until robot stops moving and starts processing the move
			while (isMoving())// || _moveRequested)
				Thread.yield();
		}

		/** Stops the current move */
		public void stopMove()
		{
			startNewMove(new Move(MoveType.STOP, 0, 0, isMoving()));
		}

		/** True, if robot is currently moving */
		public boolean isMoving()
		{
			return _moveActive;
		}

		/**
		 * Gets instance of a Move class, that represents how the robot has moved since start of the last move
		 * 
		 * @return Instance of Move representing robot's displacement since start of the last move
		 */
		public Move getDisplacement()
		{
			return new Move(hardware.getDrivenDist() - _distanceTravelledAtMoveStart,
				hardware.getAngle() - _angleRotatedAtMoveStart, isMoving());
		}

		/** Method which is called before start of any move */
		private void movementStarted()
		{
			_distanceTravelledAtMoveStart = hardware.getDrivenDist();

			_angleRotatedAtMoveStart = hardware.getAngle();
			_moveActive = true;

			// Notify listeners that new move has started
			for (MoveListener ml : _listeners)
				ml.moveStarted(_currMove, MoveHandler.this);
		}

		/** Method which is called after any move has finished */
		private void movementEnded()
		{
			_moveActive = false;

			Move displacement = getDisplacement();

			// Notify listeners that move has finished
			for (MoveListener ml : _listeners)
				ml.moveStarted(displacement, MoveHandler.this);
		}

		@Override
		public void run()
		{
			_controllerActive = true;
			while (_controllerActive)
			{
				SubOSController.mainLogger.log("Waiting for move...");
				// Waits until a new move is requested
				while (!_moveRequested && _controllerActive)
					Thread.yield();
				_moveRequested = false;

				_currMove = _requestedMove;
				_requestedMove = null;

				// If no move is requested -> stop the robot
				if (_currMove == null)
				{
					hardware.stopDriveMotors(true);
					continue;
				}

				// Copy the requested move to _currTargetMove so it can be limmited without altering
				// the requested move
				_currTargetMove = new Move(_currMove.getMoveType(), _currMove.getDistanceTraveled(),
					_currMove.getAngleTurned(), _currMove.getTravelSpeed(), _currMove.getRotateSpeed(),
					_currMove.isMoving());

				// Get a processor that is capable of processing the requested move
				MoveProcessor processor = processors.get(_currMove.getMoveType());
				// If no handler was found -> skip the request
				if (processor == null)
				{
					//TODO log error somehow (maybe add optional error logger to constructor?)
					continue;
				}

				// Prepare for handling
				processor.reset();
				movementStarted();

				SubOSController.mainLogger.log("Move started");
				while (!_moveRequested && _controllerActive)
				{
					// When a limit has changed -> update the targeted move to fit inside of the limit
					if (_limitChanged)
					{
						//@formatter:off
						_currTargetMove.setValues
						(
							_currMove.getMoveType(), 
							_distLimit < Math.abs(_currMove.getDistanceTraveled()) ? (float)_distLimit : _currMove.getDistanceTraveled(), 
							_angLimit < Math.abs(_currMove.getAngleTurned()) ? (float)_angLimit : _currMove.getAngleTurned(),
							_currMove.isMoving()
						);
						//@formatter:on

						_limitChanged = false;
					}

					// Step through the handler and if it detected that the move is at the end -> finish the move
					if (processor.step(_currTargetMove))
						break;
				}

				hardware.stopDriveMotors(!_moveRequested);
				SubOSController.mainLogger.log("Move finished. Replaced: " + _moveRequested);

				movementEnded();

				// Reset limits
				_limitChanged = false;
				_angLimit = Double.POSITIVE_INFINITY;
				_distLimit = Double.POSITIVE_INFINITY;
			}
		}
	}
}
