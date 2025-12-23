using Pip_Boy.Objects.PIP_Boy;
using System;
using System.IO;

namespace Pip_Boy
{
	/// <summary>
	/// Where the main code for the app runs
	/// </summary>
	public class Program
	{
		static void Main(string[] args)
		{
			// Default values
			string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "PIP-Boy\\");
			ConsoleColor color = ConsoleColor.DarkYellow;
			bool boot = false;

			// Parse args
			foreach (string arg in args)
			{
				if (arg.StartsWith("--folder="))
				{
					string value = arg["--folder=".Length..].Trim('"');
					if (!string.IsNullOrWhiteSpace(value))
						folderPath = value;
				}
				else if (arg.StartsWith("--color="))
				{
					string value = arg["--color=".Length..].Trim('"');
					if (Enum.TryParse<ConsoleColor>(value, true, out ConsoleColor parsedColor))
						color = parsedColor;
				}
				else if (arg.Equals("--boot", StringComparison.OrdinalIgnoreCase))
				{
					boot = true;
				}
			}

			PipBoy pipBoy = new(folderPath, color, boot);
			pipBoy.MainLoop();
		}
	}
}
