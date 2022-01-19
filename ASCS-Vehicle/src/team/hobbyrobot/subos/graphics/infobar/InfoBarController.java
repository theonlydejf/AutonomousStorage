package team.hobbyrobot.subos.graphics.infobar;

import lejos.hardware.Sound;
import lejos.hardware.lcd.*;
import lejos.utility.Delay;
import team.hobbyrobot.subos.graphics.GraphicsController;

/**
 * Trida, ktera zarizuje fungovani InfoBaru
 * 
 * @author David Krcmar
 * @version 1.0
 */
public class InfoBarController
{
	/** KONSTANTA - Velikost mezer v InfoBaru */
	public static final int VERTICAL_PADDING = 2;
	/** KONSTANTA - Vyska InfoBaru */
	private static int height = Font.getSmallFont().getHeight() + (VERTICAL_PADDING * 2) + 1;
	/** KONSTANTA - Sirka InfoBaru */
	private static final int width = GraphicsController.ScreenWidth;
	/** KONSTANTA - Delay mezi updatovanim InfoBaru */
	public static final int UPDATE_PERIOD = 50;
	
	/** FLAG - True, pokud se ma zrusit updatovani InfoBaru */
	private static boolean stopUpdating = false;

	/** Data, ktera se budou zobrazovat v InfoBaru */
	public static InfoBarData infoBarData;

	/** Thread, ktery updatuje InfoBar */
	private static Thread updateThread;
	/** GraphicsLCD InfoBaru */
	private static GraphicsLCD Graphics;

	private InfoBarController() throws Exception
	{
		throw new Exception();
	}
	
	public static int getWidth()
	{
		return width;
	}
	
	public static int getHeight()
	{
		return height;
	}

	/**
	 * Pripravi InfoBarController<br>
	 * <strong>! JE POTREBA ZAVOLAT TUTO FUNKCI HNED PO INICIALIZACI GRAPHICSCONTROLLERU !</strong>
	 */
	public static void init()
	{
		//Priprav grafiku
		Graphics = GraphicsController.getNewInfoBarGraphics();
		Graphics.setFont(Font.getSmallFont());

		//Nakresli neco na grafiku, at tam neni prazdon
		drawStartingImage();
		GraphicsController.refreshInfoBar();
	}

	/** Zacni updatovat InfoBar */
	public static void start()
	{
		if (updateThread != null && updateThread.isAlive())
			return;


		//Vytvor updatovaci Thread
		updateThread = new Thread(new Runnable()
		{
			@Override
			public void run()
			{
				//Opakuj, dokud se nema skoncit
				while (!stopUpdating)
				{
					//Updatuj Grafiku
					update();
					GraphicsController.refreshInfoBar();

					Thread.yield();
					
					//Pockej nejakou dobu
					try
					{
						Thread.sleep(UPDATE_PERIOD);
					}
					catch (InterruptedException e)
					{
						e.printStackTrace();
					}
				}
				stopUpdating = false;
			}
		});
		
		updateThread.start();
	}

	/** Prestan updatovat InfoBar */
	public static void stop()
	{
		if (!updateThread.isAlive())
			return;

		stopUpdating = true;

		while (updateThread.isAlive());
	}

	/** Updatuj InfoBar */
	public static void update()
	{
		Graphics.clear();
		drawLeftText();
		drawRightText();
		drawMiddleText();
		Graphics.setStrokeStyle(GraphicsLCD.DOTTED);
		Graphics.drawLine(0, getHeight() - 1, getWidth(), getHeight() - 1);
		Graphics.setStrokeStyle(GraphicsLCD.SOLID);
	}

	/** Nakresli levy text */
	private static void drawLeftText()
	{
		Graphics.drawString(infoBarData.getLeftText(), VERTICAL_PADDING, 3, GraphicsLCD.TOP | GraphicsLCD.LEFT);
	}

	/** Nakresli pravy text */
	private static void drawRightText()
	{
		Graphics.drawString(infoBarData.getRightText(), getWidth() - VERTICAL_PADDING, 3, GraphicsLCD.TOP | GraphicsLCD.RIGHT);
	}

	/** Nakresli text uprostred */
	private static void drawMiddleText()
	{
		Graphics.drawString(infoBarData.getMiddleText(), getWidth() / 2, 3, GraphicsLCD.TOP | GraphicsLCD.HCENTER);
	}

	/** Nakresli neco, co rika ze se InfoBar teprve zapina */
	private static void drawStartingImage()
	{
		Graphics.fillRect(0, 0, getWidth(), getHeight() - 1);
		Graphics.drawString("Starting...", getWidth() / 2, 3, GraphicsLCD.TOP | GraphicsLCD.HCENTER, true);
		Graphics.setStrokeStyle(GraphicsLCD.DOTTED);
		Graphics.drawLine(0, getHeight() - 1, getWidth(), getHeight() - 1);
		Graphics.setStrokeStyle(GraphicsLCD.SOLID);
	}
}
