package team.hobbyrobot.subos.hardware;

import java.util.ArrayList;

import lejos.hardware.Sound;
import lejos.hardware.sensor.*;
import lejos.robotics.*;
import team.hobbyrobot.subos.Referenceable;
import team.hobbyrobot.subos.SystemSound;
import team.hobbyrobot.subos.errorhandling.ErrorLogging;
import team.hobbyrobot.subos.logging.Logger;
import team.hobbyrobot.subos.menu.IncludeInRobotInfo;

//TODO: TOHLE DODELAT

/**
 * Typ ktery obsahuje veskera data o robotovy
 * 
 * @author David Krcmar
 * @version 1.0
 */
public abstract class RobotHardware
{
	/** KONSTANTA - Vzdalenost mezi stredy kol */
	@IncludeInRobotInfo
	public float WheelDistance = -1f;
	/** KONSTANTA - Polomer kol */
	@IncludeInRobotInfo
	public float WheelRadius = -1f;
	/** KONSTANTA - Touto konstantou jsou vynasobeny vsechny vzdalenosti ktere robot ujel */
	@IncludeInRobotInfo
	public float DistanceMultiplyer = (float)((Math.PI * 5.6f) / 360f);
	
	/** Promenna, ktera odkazuje na urcity senzor */
	public BaseSensor Sensor1 = null, Sensor2 = null, Sensor3 = null, Sensor4 = null;

	/** Promenna, ktera odkazuje na motory, se kteryma se jezdi */
	public EncoderMotor LeftDriveMotor = null, RightDriveMotor = null;

	/** Promenna, odkazujici na regulovane motory, ktere se daji pouzit na neo jineho (nastavce...) */
	public RegulatedMotor Motor1 = null, Motor2 = null;

	/** Meni maximalni moznou rychlost leveho motoru. Pokud je hodnota zaporna, motor bude invertovany. */
	@IncludeInRobotInfo
	public float LeftMotorPowerMultiplyer = 1;
	/** Meni maximalni moznou rychlost praveho motoru. Pokud je hodnota zaporna, motor bude invertovany. */
	@IncludeInRobotInfo
	public float RightMotorPowerMultiplyer = -1;

	/**
	 * Indexy senzoru a motoru, ktere se <strong>nemaji</strong> inicializovat<br>
	 * 0 - Port S1<br>
	 * 1 - Port S2<br>
	 * 2 - Port S3<br>
	 * 3 - Port S4<br>
	 * 4 - Port A<br>
	 * 5 - Port B<br>
	 * 6 - Port C<br>
	 * 7 - Port D
	 */
	public static final int[] HardwareIndexesToNotInitialize = new int[] {  };

	/** Abstraktni funkce, ktera ma za ukol inicializovat jeden ze senzoru */
	protected abstract void InitSensor1();

	/** Abstraktni funkce, ktera ma za ukol inicializovat jeden ze senzoru */
	protected abstract void InitSensor2();

	/** Abstraktni funkce, ktera ma za ukol inicializovat jeden ze senzoru */
	protected abstract void InitSensor3();

	/** Abstraktni funkce, ktera ma za ukol inicializovat jeden ze senzoru */
	protected abstract void InitSensor4();

	/** Abstraktni funkce, ktera ma za ukol inicializovat jeden z prebyvajicich motoru */
	public abstract void InitMotor1();

	/** Abstraktni funkce, ktera ma za ukol inicializovat jeden z prebyvajicich motoru */
	public abstract void InitMotor2();

	/** Abstraktni funkce, ktera ma za ukol inicializovat levy motor */
	public abstract void InitLeftDriveMotor();

	/** Abstraktni funkce, ktera ma za ukol inicializovat pravy motor */
	public abstract void InitRightDriveMotor();

	/** Promenna do ve ktere je ulozena instance RobotHardware, ktera se bude pouzivat */
	public static RobotHardware RobotHardwareToInitialize;

	public RobotHardware()
	{
		RobotHardwareToInitialize = this;
	}

	/**
	 * Vrati vsechny senzory v robotovi jako array
	 * 
	 * @return Senzory robota jako BaseSensor[]
	 */
	public BaseSensor[] GetSensors()
	{
		return new BaseSensor[] { Sensor1, Sensor2, Sensor3, Sensor4 };
	}

	/**
	 * Vrati vsechny motory v robotovi jako array
	 * 
	 * @return Motory robota jako BaseMotor[]
	 */
	public BaseMotor[] GetMotors()
	{
		return new BaseMotor[] { Motor1, LeftDriveMotor, RightDriveMotor, Motor2 };
	}

	/**
	 * Ziska hodnotu tachometru leveho motoru
	 * 
	 * @return Kolik ujel levy motor
	 */
	public int getLeftTacho()
	{
		int tacho = LeftDriveMotor.getTachoCount();
		if (LeftMotorPowerMultiplyer < 0)
			tacho *= -1;
		return tacho;
	}

	/**
	 * Ziska hodnotu tachometru praveho motoru
	 * 
	 * @return Kolik ujel pravy motor
	 */
	public float getRightTacho()
	{
		int tacho = RightDriveMotor.getTachoCount();
		if (LeftMotorPowerMultiplyer < 0)
			tacho *= -1;
		return tacho;
	}

	public float getLeftDrivenDist()
	{
		return getLeftTacho() * DistanceMultiplyer * Math.signum(LeftMotorPowerMultiplyer);
	}
	
	public float getRightDrivenDist()
	{
		return getRightTacho() * DistanceMultiplyer * Math.signum(RightMotorPowerMultiplyer);
	}
	
	/**
	 * Ziska kolik robot ujel od posleniho resetovani motoru
	 * 
	 * @return Cislo, rikajici jakou vzdalenost robot ujel
	 */
	public float getDrivenDist()
	{
		return (getLeftDrivenDist() + getRightDrivenDist()) / 2;
	}

	/** Resetuje tachometry drive motoru */
	public void ResetDriveMotorsTachos()
	{
		LeftDriveMotor.resetTachoCount();
		RightDriveMotor.resetTachoCount();
	}

	/**
	 * Nastavi rychlosti drive motoru
	 * 
	 * @param lSpeed Rychlost leveho motoru
	 * @param rSpeed Rychlost praveho motoru
	 */
	public void SetDrivePowers(Integer lSpeed, Integer rSpeed)
	{
		SetLeftDrivePower(lSpeed);
		SetRightDrivePower(rSpeed);
	}

	/**
	 * Nastavi rychlost leveho drive motoru
	 * 
	 * @param speed Rychlost motoru
	 */
	public void SetLeftDrivePower(Integer speed)
	{
		//aby rychlost byla v mezich motoru
		if (speed < -100)
			speed = -100;
		if (speed > 100)
			speed = 100;

		speed = (int)Math.round(speed * Math.abs(LeftMotorPowerMultiplyer));
		
		LeftDriveMotor.setPower(speed);
	}

	/**
	 * Nastavi rychlost praveho drive motoru
	 * 
	 * @param speed Rychlost motoru
	 */
	public void SetRightDrivePower(Integer speed)
	{
		//aby rychlost byla v mezich motoru
		if (speed < -100)
			speed = -100;
		if (speed > 100)
			speed = 100;

		speed = (int)Math.round(speed * Math.abs(RightMotorPowerMultiplyer));
		
		RightDriveMotor.setPower(speed);
	}

	/**
	 * Zacne otecet s drive motorama
	 * 
	 * @param forward True, pokud ma jet kladnym smerem
	 */
	public void StartDriveMotors(Boolean forward)
	{
		StartLeftDriveMotor(forward);
		StartRightDriveMotor(forward);
	}

	/**
	 * Zacne otecet s levym drive motorem
	 * 
	 * @param forward True, pokud ma jet kladnym smerem
	 */
	public void StartLeftDriveMotor(Boolean forward)
	{
		if (forward ^ (LeftMotorPowerMultiplyer < 0))
			LeftDriveMotor.forward();
		else
			LeftDriveMotor.backward();
	}

	/**
	 * Zacne otecet s pravym drive motorem
	 * 
	 * @param forward True, pokud ma jet kladnym smerem
	 */
	public void StartRightDriveMotor(Boolean forward)
	{
		if (forward ^ (RightMotorPowerMultiplyer < 0))
			RightDriveMotor.forward();
		else
			RightDriveMotor.backward();
	}

	/**
	 * Zastavi drive motory
	 * 
	 * @param hardStop Pokud true, motory zastavi prudce
	 */
	public void StopDriveMotors(Boolean hardStop)
	{
		StopLeftDriveMotor(hardStop);
		StopRightDriveMotor(hardStop);
	}

	/**
	 * Zastavi levy drive motor
	 * 
	 * @param hardStop Pokud true, motor zastavi prudce
	 */
	public void StopLeftDriveMotor(Boolean hardStop)
	{
		if (hardStop)
			LeftDriveMotor.stop();
		else
			LeftDriveMotor.flt();
	}

	/**
	 * Zastavi pravy drive motor
	 * 
	 * @param hardStop Pokud true, motor zastavi prudce
	 */
	public void StopRightDriveMotor(Boolean hardStop)
	{
		if (hardStop)
			RightDriveMotor.stop();
		else
			RightDriveMotor.flt();
	}

	/**
	 * Funkce ziska hodnotu ze senzoru
	 * 
	 * @param sensor Senzor, ze ktereho chceme ziska hodnotu
	 * @return Sample ze senzoru
	 */
	protected float[] getSample(BaseSensor sensor)
	{
		float[] smp = new float[sensor.sampleSize()];
		sensor.fetchSample(smp, 0);
		return smp;
	}

	/**
	 * <strong>Funkce delana na spousteni skrz {@link FLLMainProgram.subOS.LoadingScreen}</strong><br>
	 * Inicializuje senzory robota podle instance RobotHardware v promenne
	 * {@link #RobotHardwareToInitialize}<br>
	 * K inicializaci se pouzivaji abstraktni metody zacinajici na "Init" (napr. {@link #InitSensor1()})
	 * 
	 * @param _percentage Z kolika procent je inicializace hotova
	 * @param _msgFeed    Message feed pro LoadingScreen
	 */
	public static final void InitializeRobotHardware(Object _percentage, Object _msgFeed)
	{
		Referenceable<Float> percentage = (Referenceable<Float>) _percentage;
		final ArrayList<String> msgFeed = (ArrayList<String>) _msgFeed;

		//Thready ve kterych se budou inicializovat senzory
		Thread[] initThreads = new Thread[8];

		//Funkce pro inicializace
		// @formatter:off
		final Runnable[] initFuncs = new Runnable[]
		{ 
			new Runnable() { public void run() { RobotHardwareToInitialize.InitSensor1(); }},
			new Runnable() { public void run() { RobotHardwareToInitialize.InitSensor2(); }},
			new Runnable() { public void run() { RobotHardwareToInitialize.InitSensor3(); }},
			new Runnable() { public void run() { RobotHardwareToInitialize.InitSensor4(); }},
			
			new Runnable() { public void run() { RobotHardwareToInitialize.InitMotor1(); }},
			new Runnable() { public void run() { RobotHardwareToInitialize.InitLeftDriveMotor(); }},
			new Runnable() { public void run() { RobotHardwareToInitialize.InitRightDriveMotor(); }},
			new Runnable() { public void run() { RobotHardwareToInitialize.InitMotor2(); }}
		};
		// @formatter:on

		//Inicializuj thready
		for (int i = 0; i < initThreads.length; i++)
		{
			final int _i = i;
			//Funkce ktera pobezi v threadu
			Runnable threadAction = new Runnable()
			{
				@Override
				public void run()
				{
					for (int i : HardwareIndexesToNotInitialize)
					{
						if (i == _i)
							return;
					}

					Exception finalException = new Exception();
					//Zkus 3x inicializovat senzor
					for (int j = 0; j < 3; j++)
					{
						try
						{
							//Spusti inicializacni funkci
							initFuncs[_i].run();
							//Dej najevo ze se senzor inicializoval
							msgFeed.add("harInit ID: " + _i + ": DONE!");
							Sound.playTone(1000, 150, 10);
							return;
						}
						catch (Exception e)
						{
							//
							//Pokud se inicializace nepovedla
							SystemSound.playNonFatalErrorSound(true);
							//
							//Pokud se neco nepovede
							//Rekni to v msg feedu
							msgFeed.add("harInit ERR; HAR ID: " + _i);
							//Blikni
							BrickHardware.blinkLED(2, 300, true, 1);
							//Loguj chybu
							ErrorLogging.logError(
								"Error when loading; Attempt number:" + j + ";" + Logger.getExceptionInfo(e));
						}
					}

					//Pokud se inicializace 3x nepovedla -> FATAL ERROR
					ErrorLogging.logFatalError(Logger.getExceptionInfo(finalException));
					ErrorLogging.startErrorLogScreen();

				}
			};

			//Vytvor a spust thread
			initThreads[i] = new Thread(threadAction);
			initThreads[i].start();
		}

		ArrayList<Integer> finishedIndexes = new ArrayList<Integer>();
		boolean allThreadsFinished = false;

		while (true)
		{
			allThreadsFinished = true;

			//Zkontroluj jestli vsechny thready dobehli
			for (int i = 0; i < initThreads.length; i++)
			{
				if (finishedIndexes.contains(i))
					continue;
				if (initThreads[i].isAlive())
				{
					allThreadsFinished = false;
				}
				else
				{
					finishedIndexes.add(i);
				}
			}

			//Rekni z kolika procent je inicializace hotova
			percentage.setValue((float) finishedIndexes.size() / 8f);

			//Pokud jsou vsechny thready hotove -> KONEC
			if (allThreadsFinished)
				break;
		}

		AfterHardwareInitialize();
	}

	public static void AfterHardwareInitialize()
	{

	}
}
