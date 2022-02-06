package team.hobbyrobot.subos.hardware.motor;

import lejos.hardware.motor.BaseRegulatedMotor;
import lejos.hardware.motor.MotorRegulator;
import lejos.hardware.port.Port;
import lejos.hardware.port.TachoMotorPort;
import lejos.hardware.sensor.EV3SensorConstants;
import lejos.robotics.EncoderMotor;

/**
 * Abstraction for a Medium Lego EV3/NXT motor.
 * 
 */
public class EV3DCMediumRegulatedMotor extends BaseRegulatedMotor implements EncoderMotor
{
	public static float MOVE_P = 8f;
	public static float MOVE_I = 0.04f;
	public static float MOVE_D = 8f;
	public static float HOLD_P = 8f;
	public static float HOLD_I = 0.02f;
	public static float HOLD_D = 0f;
	public static int OFFSET = 1000;

	private static final int MAX_SPEED = 260 * 360 / 60;

	private int power;
	
	/**
	 * Use this constructor to assign a variable of type motor connected to a particular port.
	 * 
	 * @param port to which this motor is connected
	 */
	public EV3DCMediumRegulatedMotor(TachoMotorPort port)
	{
		super(port, null, EV3SensorConstants.TYPE_MINITACHO, MOVE_P, MOVE_I, MOVE_D, HOLD_P, 
			HOLD_I, HOLD_D, OFFSET, MAX_SPEED);
	}

	/**
	 * Use this constructor to assign a variable of type motor connected to a particular port.
	 * 
	 * @param port to which this motor is connected
	 */
	public EV3DCMediumRegulatedMotor(Port port)
	{
        super(port, null, EV3SensorConstants.TYPE_NEWTACHO, MOVE_P, MOVE_I, MOVE_D,
            HOLD_P, HOLD_I, HOLD_D, OFFSET, MAX_SPEED);
	}
	
	public void backwardUnregulated()
	{
		if(reg.isMoving())
			suspendRegulation();
		
		tachoPort.controlMotor(power, TachoMotorPort.BACKWARD);
	}
	
	public void forwardUnregulated()
	{
		if(reg.isMoving())
			suspendRegulation();
		
		tachoPort.controlMotor(power, TachoMotorPort.FORWARD);
	}
	
	@Override
	public void setPower(int power)
	{
		this.power = power;
		setSpeed(getMaxSpeed() * (power / 100f));
	}

	@Override
	public int getPower()
	{
		// TODO Auto-generated method stub
		return power;
	}

	public MotorRegulator getRegulator()
	{
		return tachoPort.getRegulator();
	}
}
