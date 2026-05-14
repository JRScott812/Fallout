using Pip_Boy_Classes.Entities;
using Pip_Boy_Classes.Objects;
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
			bool boot = true;

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
					if (Enum.TryParse(value, true, out ConsoleColor parsedColor))
						color = parsedColor;
				}
				else if (arg.Equals("--boot", StringComparison.OrdinalIgnoreCase))
				{
					boot = true;
				}
			}

			folderPath = Path.GetFullPath(folderPath);

			// Ensure a Player exists in the configured game folder; if not, prompt the user to create one.
			try
			{
				string inventoryDir = folderPath;
				string playerFilePath = Path.Combine(inventoryDir, "Player.xml");
				bool playerExists = File.Exists(playerFilePath);

				if (!playerExists)
				{
					Console.Write("No player found. Create a default player now? (Y/n): ");
					string? resp = Console.ReadLine();
					if (string.IsNullOrWhiteSpace(resp) || resp.StartsWith("y", StringComparison.OrdinalIgnoreCase))
					{
						Directory.CreateDirectory(inventoryDir);
						// Ask for player name
						Console.Write("Enter player name [Player]: ");
						string? playerName = Console.ReadLine();
						if (string.IsNullOrWhiteSpace(playerName))
							playerName = "Player";

						// Prompt for SPECIAL values (Strength, Perception, Endurance, Charisma, Intelligence, Agility, Luck)
						string[] specialNames = ["Strength", "Perception", "Endurance", "Charisma", "Intelligence", "Agility", "Luck"];
						byte[] attributeValues = new byte[specialNames.Length];
						Console.WriteLine("Set SPECIAL values (1-10). Use ←/→ or +/- to change, Enter to accept each value.");

						// Local helper to read a value using arrow keys
						static byte ReadAttributeWithArrows(string name, byte defaultValue)
						{
							int val = defaultValue;
							string prompt = $"{name} [{val}] ";
							Console.Write(prompt);
							while (true)
							{
								ConsoleKeyInfo keyInfo = Console.ReadKey(true);
								switch (keyInfo.Key)
								{
									case ConsoleKey.LeftArrow:
									case ConsoleKey.OemMinus:
									case ConsoleKey.Subtract:
										if (val > 1) val--;
										break;
									case ConsoleKey.RightArrow:
									case ConsoleKey.OemPlus:
									case ConsoleKey.Add:
										if (val < 10) val++;
										break;
									case ConsoleKey.UpArrow:
										if (val < 10) val++;
										break;
									case ConsoleKey.DownArrow:
										if (val > 1) val--;
										break;
									case ConsoleKey.Enter:
										Console.WriteLine();
										return (byte)val;
									case ConsoleKey.D1:
									case ConsoleKey.D2:
									case ConsoleKey.D3:
									case ConsoleKey.D4:
									case ConsoleKey.D5:
									case ConsoleKey.D6:
									case ConsoleKey.D7:
									case ConsoleKey.D8:
									case ConsoleKey.D9:
									case ConsoleKey.D0:
										int parsedDigit = (int)char.GetNumericValue(keyInfo.KeyChar);
										if (parsedDigit is >= 1 and <= 10)
										{
											val = parsedDigit == 0 ? 10 : parsedDigit; // treat 0 as 10
										}
										break;
									default:
										break;
								}

								// Update the prompt display on the same line
								string display = $"{name} [{val}] ";
								Console.Write("\r" + display.PadRight(prompt.Length));
							}
						}

						for (int i = 0; i < specialNames.Length; i++)
						{
							attributeValues[i] = ReadAttributeWithArrows(specialNames[i], 1);
						}

						// Create Player with supplied SPECIAL values and serialize
						Player player = new(playerName, attributeValues, inventoryDir);
						PipBoySerializer.ToFile(inventoryDir, player, "Player.xml");
						Console.WriteLine("Player created at: " + Path.Combine(inventoryDir, "Player.xml"));
					}
				}
			}
			catch (Exception ex)
			{
				// Non-fatal: continue even if we couldn't create the player automatically.
				Console.WriteLine("Warning: could not ensure Player.xml exists: " + ex.Message);
			}

			PipBoy pipBoy = new(folderPath, color, boot);
			pipBoy.MainLoop();
		}
	}
}
