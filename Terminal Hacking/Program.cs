using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Terminal_Minigame
{
	/// <summary>
	/// Main program for the terminal hacking minigame.
	/// </summary>
	internal class Program
	{
		/// <summary>
		/// The dud remover pairs
		/// </summary>
		public static readonly string[] dudRemovers = ["<>", "[]", "()", "{}"];
		/// <summary>
		/// The allowed gibberish characters
		/// </summary>
		public static readonly char[] gibberish = [';', ':', '%', '$', '@', '=', '~', '_', '\\', '/', '-', '+', '?', '#', '^', '`', '\'', '\"'];

		/// <summary>
		/// Where all the symbols go.  Grid is 12 * 17 * 2 chars long
		/// </summary>
		public static char[] grid = new char[408];

		/// <summary>
		/// Password guess attempts left 
		/// </summary>
		public static byte attemptsLeft = 4;

		/// <summary>
		/// Index of selected sequence
		/// </summary>
		public static ushort selectionIndex = 0;

		/// <summary>
		/// The selected sequence to check
		/// </summary>
		public static string selectedSequence = string.Empty;

		/// <summary>
		/// The correct word
		/// </summary>
		public static string correctWord = string.Empty;

		public static readonly Random random = new();
		public static readonly WordBank wordBank = new();

		/// <summary>
		/// Main entry point for the terminal hacking minigame.
		/// </summary>
		static void Main()
		{
			Console.ForegroundColor = ConsoleColor.Green;

			string dataEntriesPath = Path.Combine(AppContext.BaseDirectory, "Data Entries");
			string[] dataEntryFilePaths = Directory.GetFiles(dataEntriesPath, "*.txt");
			Data[] dataEntries = new Data[dataEntryFilePaths.Length];

			for (int i = 0; i < dataEntryFilePaths.Length; i++)
			{
				dataEntries[i] = new(dataEntryFilePaths[i]);
			}

			Console.WriteLine("Select Difficulty:");
			Console.WriteLine("\t1: Very Easy");
			Console.WriteLine("\t2: Easy");
			Console.WriteLine("\t3: Average");
			Console.WriteLine("\t4: Hard");
			Console.WriteLine("\t5: Very Hard");

			ConsoleKey pressedKey = Console.ReadKey().Key;

			// Difficulty Selection
			switch (pressedKey)
			{
				case ConsoleKey.D1 or ConsoleKey.NumPad1:
					break;
				case ConsoleKey.D2 or ConsoleKey.NumPad2:
					break;
				case ConsoleKey.D3 or ConsoleKey.NumPad3:
					break;
				case ConsoleKey.D4 or ConsoleKey.NumPad4:
					break;
				case ConsoleKey.D5 or ConsoleKey.NumPad5:
					break;
			}

			WriteFile();

			#region Boot Screen
			Console.Clear();
			SlowType("WELCOME TO ROBCO INDUSTRIES (TM) TERMLINK");
			Console.WriteLine();
			SlowType(">SET TERMINAL/INQUIRE");
			Console.WriteLine();
			SlowType("RIT-V300");
			Console.WriteLine();
			SlowType(">SET FILE/PROTECTION=OWNER:RWED ACCOUNTS.F");
			SlowType(">SET HALT RESTART/MAINT");
			Console.WriteLine();
			SlowType("Initializing Robco Industries(TM) MF Boot Agent v2.3.0");
			SlowType("RETROS BIOS");
			SlowType("RBIOS-4.02.08.00 52EE5.E7.E8");
			SlowType("Copyright 2201-2203 Robco Ind.");
			SlowType("Uppermem: 64 KB");
			SlowType("Root (5A8)");
			SlowType("Maintenance Mode");
			Console.WriteLine();
			SlowType(">RUN DEBUG/ACCOUNTS.F");
			Console.Clear();
			#endregion

			ReadFile();

			#region Main Loop
			while (attemptsLeft > 0)
			{
				pressedKey = Console.ReadKey().Key;
				switch (pressedKey)
				{
					#region Navigation
					case ConsoleKey.UpArrow or ConsoleKey.W:
						selectionIndex++;
						break;
					case ConsoleKey.DownArrow or ConsoleKey.S:
						selectionIndex--;
						break;
					case ConsoleKey.LeftArrow or ConsoleKey.A:
						selectionIndex--;
						break;
					case ConsoleKey.RightArrow or ConsoleKey.D:
						selectionIndex++;
						break;
					#endregion

					#region Selection
					case ConsoleKey.Enter:
						Console.WriteLine($"Invalid {Compare(selectedSequence, correctWord)}/{correctWord.Length} match.");
						break;
						#endregion
				}
				Console.Clear();
			}
			#endregion
		}

		/// <summary>
		/// Compares how similar the guess and answer are
		/// </summary>
		/// <param name="guess">The guessed sequence</param>
		/// <param name="word">The correct sequence</param>
		/// <returns>The number of matching characters.</returns>
		static byte Compare(string guess, string word)
		{
			byte similarity = 0;
			if (guess == word)
			{
				return (byte)word.Length;
			}
			for (int i = 0; i < word.Length; i++)
			{
				if (word.Contains(guess[i]))
				{
					similarity++;
				}
			}
			return similarity;
		}

		/// <summary>
		/// Slowly types out text character-by-character with a delay.
		/// </summary>
		/// <param name="text">The text to display.</param>
		static void SlowType(string text)
		{
			foreach (char character in text)
			{
				Console.Write(character);
				Thread.Sleep(10);
			}
		}

		/// <summary>
		/// Writes a test file containing random gibberish and words for the hacking minigame.
		/// </summary>
		static void WriteFile()
		{
			FileStream fileStream = new(".\\ACCOUNTS.F", FileMode.Create, FileAccess.Write);

			byte[] byteArray;

			string[] words = ["SECRET", "BRAVO", "SECURITY", "HACKS", "PASSWORD", "FILE"];

			foreach (string word in words)
			{
				byteArray = GenerateGibberish(random.Next(byte.MaxValue));
				fileStream.Write(byteArray);
				fileStream.Write(StringToByteArray(word));
			}

			fileStream.Close();
		}

		/// <summary>
		/// Reads and displays the contents of the ACCOUNTS.F file in a hex dump format.
		/// </summary>
		static void ReadFile()
		{
			FileStream fileStream = new(".\\ACCOUNTS.F", FileMode.Open, FileAccess.Read);

			byte[] buffer = new byte[32];
			long address = 0;
			int bytesRead;

			while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
			{
				// Print the address and ASCII representation for the first half
				Console.Write("{0:X6}  ", address);
				PrintAscii(buffer, 0, bytesRead / 2);
				Console.Write("    ");

				// Print the address and ASCII representation for the second half
				Console.Write("{0:X6}  ", address + (bytesRead / 2));
				PrintAscii(buffer, bytesRead / 2, bytesRead);

				Console.WriteLine();

				address += bytesRead;
			}

			fileStream.Close();
		}

		/// <summary>
		/// Converts a string to a byte array.
		/// </summary>
		/// <param name="stringToTranslate">The string to convert.</param>
		/// <returns>A byte array representation of the string.</returns>
		static byte[] StringToByteArray(string stringToTranslate)
		{
			byte[] tempArray = new byte[stringToTranslate.Length];
			for (int i = 0; i < tempArray.Length; i++)
			{
				tempArray[i] = (byte)stringToTranslate[i];
			}
			return tempArray;
		}

		/// <summary>
		/// Generates random gibberish characters for the hacking minigame display.
		/// </summary>
		/// <param name="length">The number of gibberish characters to generate.</param>
		/// <returns>A byte array containing random gibberish characters.</returns>
		static byte[] GenerateGibberish(int length)
		{
			byte[] tempArray = new byte[length];
			for (int i = 0; i < tempArray.Length; i++)
			{
				tempArray[i] = (byte)gibberish[random.Next(gibberish.Length)];
			}
			return tempArray;
		}

		/// <summary>
		/// Prints ASCII representation of bytes in the buffer, replacing non-printable characters with a period.
		/// </summary>
		/// <param name="buffer">The byte buffer to read from.</param>
		/// <param name="start">The starting index in the buffer.</param>
		/// <param name="end">The ending index in the buffer.</param>
		static void PrintAscii(byte[] buffer, int start, int end)
		{
			for (int i = start; i < end; i++)
			{
				if (buffer[i] < 32 || buffer[i] > 126) // non-printable character
				{
					Console.Write('.');
				}
				else
				{
					Console.Write(Encoding.ASCII.GetString(buffer, i, 1));
				}
			}
		}
	}
}