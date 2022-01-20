package team.hobbyrobot.ascsvehicle.exe;

import lejos.hardware.Battery;
import lejos.hardware.Sound;
import team.hobbyrobot.ascsvehicle.ASCSVehicleHardware;
import team.hobbyrobot.subos.SubOSController;
import team.hobbyrobot.subos.graphics.infobar.BasicInfoBar;
import team.hobbyrobot.subos.hardware.BrickHardware;
import team.hobbyrobot.subos.hardware.LEDBlinkingStyle;
import team.hobbyrobot.subos.menu.MenuItem;
import team.hobbyrobot.subos.menu.MenuScreen;
import team.hobbyrobot.subos.menu.RobotInfoScreen;

public class ASCSVehicleRun
{
	//@formatter:off
		public static final MenuItem[] MainMenu = new MenuItem[] 
		{
			new RobotInfoScreen(ASCSVehicleHardware.class)
		};
		//@formatter:on

		/**  Inicializovany Hardware robota */
		public static ASCSVehicleHardware RobotHardware = new ASCSVehicleHardware(-1, -1, -1);
		/** Inicializovany InfoBar, ktery aktualne bezi */
		public static BasicInfoBar InfoBar = null;

		public static void main(String[] args) throws Exception
		{
			
			//Inicializuj senzory v robotovi a subOS
			InfoBar = SubOSController.init(RobotHardware, BasicInfoBar.class);
			
			//Dej najevo, že robot už je připraven k použití
			BrickHardware.setLEDPattern(1, LEDBlinkingStyle.NONE, 0);
			Sound.beepSequenceUp();
			
			//Spust menu a opakuj ho do nekonecna
			while (true)
			{
				MenuScreen mainMenu = new MenuScreen(MainMenu);
				mainMenu.select();
			}
		}
}
