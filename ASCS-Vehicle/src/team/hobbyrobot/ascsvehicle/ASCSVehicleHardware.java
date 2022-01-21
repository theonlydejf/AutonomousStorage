package team.hobbyrobot.ascsvehicle;

import java.util.ArrayList;

import lejos.hardware.Button;
import lejos.hardware.Sound;
import lejos.hardware.motor.EV3MediumRegulatedMotor;
import lejos.hardware.motor.UnregulatedMotor;
import lejos.hardware.port.Port;
import lejos.hardware.sensor.BaseSensor;
import lejos.robotics.EncoderMotor;
import lejos.robotics.RegulatedMotor;
import lejos.utility.Delay;
import lejos.utility.Stopwatch;
import team.hobbyrobot.subos.Referenceable;
import team.hobbyrobot.subos.SubOSController;
import team.hobbyrobot.subos.hardware.RobotHardware;
import team.hobbyrobot.subos.hardware.motor.EV3DCMediumRegulatedMotor;
import team.hobbyrobot.subos.menu.IncludeInRobotInfo;

public class ASCSVehicleHardware extends RobotHardware
{
	@IncludeInRobotInfo
	public int LifterUp = 0;
	
	public ASCSVehicleHardware(float wheelDistance, float wheelRadius, float distanceMultiplier)
	{
		super(wheelDistance, wheelRadius, distanceMultiplier);
		// TODO Auto-generated constructor stub
	}

	public void moveLifterTo(int percent)
	{
		Motor1.rotateTo((int)(LifterUp * (percent / 100f))); 
	}
	
	public void fltLifter()
	{
		Motor1.flt();
	}
	
	public void stopLifter()
	{
		Motor1.stop();
	}
	
	@Override
	protected BaseSensor initSensor1(Port port)
	{
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	protected BaseSensor initSensor2(Port port)
	{
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	protected BaseSensor initSensor3(Port port)
	{
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	protected BaseSensor initSensor4(Port port)
	{
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	protected RegulatedMotor initMotor1(Port port)
	{
		// TODO Auto-generated method stub
		return new EV3DCMediumRegulatedMotor(port);
	}

	@Override
	protected RegulatedMotor initMotor2(Port port)
	{
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	protected EncoderMotor initLeftDriveMotor(Port port)
	{
		// TODO Auto-generated method stub
		return new EV3DCMediumRegulatedMotor(port);
	}

	@Override
	protected EncoderMotor initRightDriveMotor(Port port)
	{
		// TODO Auto-generated method stub
		return new EV3DCMediumRegulatedMotor(port);
	}
	
	public void calibrateLifter()
	{
		calibrateLifter(new Referenceable<Float>(0f), new ArrayList<String>());
	}
	
	public void calibrateLifter(Object _percentage, Object _msgFeed)
	{
		Referenceable<Float> percentage = (Referenceable<Float>) _percentage;
		final ArrayList<String> msgFeed = (ArrayList<String>) _msgFeed;
		
		// Home lifting mechanism
		SubOSController.mainLogger.log("homing lifter...");
		Motor1.setSpeed(360);
		Motor1.setStallThreshold(5, 200);
		Motor1.forward();
		Stopwatch sw = new Stopwatch();
		while(!Motor1.isStalled() && sw.elapsed() < 750)
			Thread.yield();
		Motor1.flt();
		SubOSController.mainLogger.log("lifter home!");
		Sound.playTone(1000, 50);
		
		// Wait a bit so motor can move to its resting position
		Delay.msDelay(500);
		Motor1.resetTachoCount();
		
		percentage.setValue(.3f);
		
		// Move lifting mechanism to the other side
		SubOSController.mainLogger.log("moving lifter to far end...");
		sw.reset();
		Motor1.backward();
		Delay.msDelay(1000);
		Motor1.flt();
		percentage.setValue(.5f);
		SubOSController.mainLogger.log("lifter at far end!");
		Sound.playTone(1000, 50);
		
		// Wait a bit so motor can move to its resting position
		SubOSController.mainLogger.log("waiting...");
		Delay.msDelay(500);
		
		percentage.setValue(.6f);
		
		// Calculate the middle, ie the highest point of the lifting mechanism
		LifterUp = Motor1.getTachoCount() / 2;
		percentage.setValue(.9f);
		
		// Home the lifting mechanism
		SubOSController.mainLogger.log("homing lifter...");
		Motor1.rotateTo(0);
		Motor1.flt();

		// Move lifter up and down to make sure it works
		Motor1.setSpeed((int) Motor1.getMaxSpeed());
		
		percentage.setValue(1f);
		msgFeed.add("lifter calib: done!");
	}
	
	public static void calibrateDefaultLifter(Object _percentage, Object _msgFeed)
	{
		((ASCSVehicleHardware)RobotHardware.RobotHardwareToInitialize).calibrateLifter(_percentage, _msgFeed);
	}
}
