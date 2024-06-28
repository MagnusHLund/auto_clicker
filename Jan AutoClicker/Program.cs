using System;
using System.IO;
using System.Threading;
using WindowsInput;
using WindowsInput.Native;

class Program
{
	static string filePath = "MagnusLund/AutoClicker/";
	static string fileName = "Config.txt";
	static string fullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), filePath, fileName);

	static void Main(string[] args)
	{
		if (!File.Exists(fullPath))
		{
			CreateFile();
		}

		string[] settings = LoadFile();

		string button = settings[0];
		VirtualKeyCode toggleKey = (VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), button, true);
		double clickInterval = Convert.ToDouble(settings[1]);
		double clickDuration = Convert.ToDouble(settings[2]);

		var inputSimulator = new InputSimulator();
		bool autoClickerOn = false;

		Console.WriteLine("You can now enable/disable the autoclicker, using " + settings[0]);
		Console.WriteLine($"Click happens every {settings[1]} secs\nClick length is {settings[2]} secs");
		Console.WriteLine($"Change these settings in {fullPath}");

		while (true)
		{
			if (inputSimulator.InputDeviceState.IsKeyDown(toggleKey))
			{
				autoClickerOn = !autoClickerOn;
				Thread.Sleep(500); // To prevent multiple toggles in quick succession
			}

			if (autoClickerOn)
			{
				inputSimulator.Mouse.LeftButtonDown();
				SleepWithKeyCheck(clickDuration, toggleKey, ref autoClickerOn);
				inputSimulator.Mouse.LeftButtonUp();
				SleepWithKeyCheck(clickInterval, toggleKey, ref autoClickerOn);
			}
			else
			{
				Thread.Sleep(100); // Sleep to prevent high CPU usage
			}
		}
	}

	static void SleepWithKeyCheck(double seconds, VirtualKeyCode key, ref bool flag)
	{
		double elapsed = 0;
		var inputSimulator = new InputSimulator(); // Create an instance of InputSimulator

		while (elapsed < seconds)
		{
			Thread.Sleep(100); // Sleep for 100 ms increments
			elapsed += 0.1;

			// Call IsKeyDown on the inputSimulator instance
			if (inputSimulator.InputDeviceState.IsKeyDown(key))
			{
				flag = !flag;
				Thread.Sleep(500); // To prevent multiple toggles in quick succession
			}
		}
	}


	static void CreateFile()
	{
		Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

		using (StreamWriter writer = File.CreateText(fullPath))
		{
			Console.WriteLine("The following information will be saved:");
			Console.WriteLine("Enter the key to activate/deactivate the autoclicker (e.g., F10):");
			writer.WriteLine(Console.ReadLine());

			Console.WriteLine("Enter how often it should click in seconds (e.g. 5.32):");
			writer.WriteLine(Console.ReadLine());

			Console.WriteLine("Enter how long the click should last in seconds:");
			writer.WriteLine(Console.ReadLine());
		}
	}

	static string[] LoadFile()
	{
		string[] settings = new string[3];
		using (StreamReader reader = File.OpenText(fullPath))
		{
			settings[0] = reader.ReadLine();
			settings[1] = reader.ReadLine();
			settings[2] = reader.ReadLine();
		}
		return settings;
	}
}
