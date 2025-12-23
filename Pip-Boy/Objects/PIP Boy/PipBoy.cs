using Pip_Boy.Data_Types;
using Pip_Boy.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pip_Boy.Objects.PIP_Boy
{
	/// <summary>
	/// Displays <see cref="Player"/> info, <see cref="Inventory"/>, and other data.
	/// It controls output and error handling through composed components.
	/// </summary>
	public class PipBoy
	{
		#region Components
		/// <summary>
		/// Handles all UI display logic.
		/// </summary>
		private readonly PipBoyUI _ui;

		/// <summary>
		/// Handles all audio playback.
		/// </summary>
		private readonly PipBoyAudio _audio;

		/// <summary>
		/// Handles all user input processing.
		/// </summary>
		private readonly PipBoyInput _input;
		#endregion

		#region System Info
		/// <summary>
		/// The Current <see cref="DateOnly"/> and <see cref="TimeOnly"/>.
		/// </summary>
		public DateTime DateTime { get; private set; }
		#endregion

		#region Objects
		/// <summary>
		/// The <c>Player</c> object tied to the PIP-Boy.
		/// </summary>
		public Player Player { get; private set; }

		/// <summary>
		/// Controls music.
		/// </summary>
		public Radio Radio { get; private set; }

		/// <summary>
		/// Displays points of interest.
		/// </summary>
		public Map Map { get; private set; }
		#endregion

		#region Lists
		/// <summary>
		/// A list of all unfinished quests, it can be added to, and quests will be removed and added to the `finishedQuests`, once finished.
		/// </summary>
		public List<Quest> Quests { get; } = [];

		/// <summary>
		/// A list of all finished quests, which will grow.
		/// </summary>
		public List<Quest> FinishedQuests { get; } = [];

		/// <summary>
		/// An array of all factions.  Descriptions are taken from the Fallout Wiki (phrasing breaks immersion).  I might change these later.
		/// </summary>
		public Faction[] Factions { get; private set; } = [];

		/// <summary>
		/// The current index of the selected <see cref="Faction"/> in the <c>General</c> sub page of the <c>STAT</c> page
		/// </summary>
		public byte FactionIndex { get; set; } = 0;
		#endregion

		#region Directories
		/// <summary>
		/// The directory from which files will be loaded and saved
		/// </summary>
		public string ActiveDirectory { get; }

		/// <summary>
		/// The directory from which data files will be loaded and saved
		/// </summary>
		public string DataDirectory { get; }

		/// <summary>
		/// The directory from which factions will be loaded and saved
		/// </summary>
		public string FactionDirectory { get; }
		#endregion

		/// <summary>
		/// The path of the player's <c>*.xml</c> file.
		/// </summary>
		public string PlayerFilePath { get; private set; }

		#region Constructors
		/// <param name="workingDirectory">The directory to load items, sounds, songs, and player info from</param>
		/// <param name="color">The display color</param>
		/// <param name="boot">Whether to show the boot screen.</param>
		public PipBoy(string workingDirectory, ConsoleColor color, bool boot)
		{
			DataDirectory = Path.Combine(workingDirectory, "Data") + Path.DirectorySeparatorChar;
			FactionDirectory = Path.Combine(workingDirectory, "Factions") + Path.DirectorySeparatorChar;

			// Initialize components
			_audio = new PipBoyAudio(workingDirectory);
			_ui = new PipBoyUI(this, color);
			_input = new PipBoyInput(this, _audio);

			// Load factions
			string[] filePaths = Directory.GetFiles(FactionDirectory, "*.txt");
			Factions = new Faction[filePaths.Length];
			for (int i = 0; i < filePaths.Length; i++)
			{
				Factions[i] = new(Path.GetFileNameWithoutExtension(filePaths[i]), File.ReadAllText(filePaths[i]));
			}

			Radio = new(Path.Combine(workingDirectory, "Songs") + Path.DirectorySeparatorChar);
			Map = new(25, 50, Path.Combine("PIP-Boy", "Map Locations") + Path.DirectorySeparatorChar);

			ActiveDirectory = workingDirectory;
			DateTime = System.DateTime.Now;

			Console.ForegroundColor = color;
			Console.Title = "PIP-Boy 3000 MKIV";
			Console.OutputEncoding = Encoding.UTF8;

			if (boot)
			{
				_audio.PlayBootSequence();
			}

			try
			{
				PlayerFilePath = Directory.GetFiles(ActiveDirectory, "*.xml")[0];
				Player = PipBoySerializer.FromFile<Player>(PlayerFilePath);
				Player.Inventory = new(Path.Combine(ActiveDirectory, "Inventory") + Path.DirectorySeparatorChar, Player);
			}
			catch (IndexOutOfRangeException)
			{
				Player = Player.CreatePlayer(ActiveDirectory);
			}

			Map.MovePlayer(null, null, Player);
		}
		#endregion

		#region Page Info
		/// <summary>
		/// The current main page
		/// </summary>
		public Pages currentPage = Pages.STATS;

		/// <summary>
		/// The current STAT sub-page
		/// </summary>
		public StatsPages statPage = StatsPages.Status;

		/// <summary>
		/// The current DATA sub-page
		/// </summary>
		public DataPages dataPage = DataPages.Map;
		#endregion

		#region Menu Navigation
		/// <summary>
		/// The main loop that controls the PIP-Boy with keyboard input
		/// </summary>
		public void MainLoop()
		{
			ConsoleKey key = ConsoleKey.Escape;
			while (key != ConsoleKey.Q)
			{
				Console.Clear();

				_ui.Highlight(currentPage.ToString(), true);
				Console.WriteLine();

				Console.WriteLine(_ui.ShowMenu(currentPage, statPage, dataPage));
				Console.WriteLine();

				_ui.ShowSubMenu(PipBoyUI.GetSubMenu(currentPage), statPage, dataPage);

				key = Console.ReadKey(true).Key;

				switch (key)
				{
					#region Menu
					case ConsoleKey.A:
						_input.ChangeMenu(false, ref currentPage);
						break;
					case ConsoleKey.D:
						_input.ChangeMenu(true, ref currentPage);
						break;
					#endregion

					#region Sub-Menu
					case ConsoleKey.LeftArrow:
						_input.ChangeSubMenu(false, currentPage, ref statPage, ref dataPage);
						break;
					case ConsoleKey.RightArrow:
						_input.ChangeSubMenu(true, currentPage, ref statPage, ref dataPage);
						break;

					case ConsoleKey.UpArrow when currentPage == Pages.STATS && statPage == StatsPages.General:
						_input.ChangeSelectedFaction(false);
						break;
					case ConsoleKey.DownArrow when currentPage == Pages.STATS && statPage == StatsPages.General:
						_input.ChangeSelectedFaction(true);
						break;
					#endregion

					#region Radio
					case ConsoleKey.Enter when currentPage == Pages.DATA && dataPage == DataPages.Radio:
						Radio.Play();
						break;

					case ConsoleKey.Add when currentPage == Pages.DATA && dataPage == DataPages.Radio:
						Radio.AddSong(this);
						break;

					case ConsoleKey.UpArrow when Radio.songIndex > 0:
						Radio.ChangeSong(false);
						Radio.Play();
						break;
					case ConsoleKey.DownArrow when Radio.songIndex < Radio.songs.Length:
						Radio.ChangeSong(true);
						Radio.Play();
						break;
					#endregion

					#region Map
					case ConsoleKey.NumPad8 when currentPage == Pages.DATA && dataPage == DataPages.Map:
						Map.MovePlayer(true, null, Player);
						break;
					case ConsoleKey.NumPad2 when currentPage == Pages.DATA && dataPage == DataPages.Map:
						Map.MovePlayer(false, null, Player);
						break;
					case ConsoleKey.NumPad4 when currentPage == Pages.DATA && dataPage == DataPages.Map:
						Map.MovePlayer(null, false, Player);
						break;
					case ConsoleKey.NumPad6 when currentPage == Pages.DATA && dataPage == DataPages.Map:
						Map.MovePlayer(null, true, Player);
						break;
					#endregion

					#region Object Creation (for testing)
					case ConsoleKey.L:
						Console.Write("Enter the type of object to create (e.g., Weapon, Player, Ghoul): ");
						string enteredType = Console.ReadLine();
						Type type = PipBoySerializer.GetSerializableTypeByName(enteredType);
						object createdObject = PipBoyInput.CreateFromInput(type);
						string filePath = PipBoySerializer.ToFile(Directory.GetCurrentDirectory(), createdObject);
						Console.WriteLine($"Created object of type {type.Name} and saved to file: {filePath}");
						Console.WriteLine($"Press {ConsoleKey.Enter} to continue: ");
						Console.ReadLine();
						break;
						#endregion
				}
			}
			Shutdown();
		}
		#endregion

		#region Shutdown
		/// <summary>
		/// Write all data to files and play shutdown sequence.
		/// </summary>
		private void Shutdown()
		{
			_audio.PlayShutdownSequence();

			Player.SavePlayerPerks();
			Player.Inventory.Save();
			PipBoySerializer.ToFile(ActiveDirectory, Player);
		}
		#endregion

		#region Public API
		/// <summary>
		/// Displays an error message to the user.
		/// </summary>
		/// <param name="message">The error message to display</param>
		public void Error(string message)
		{
			_ui.Error(message);
		}
		#endregion

		#region Enums
		/// <summary>
		/// Main Menu pages which display different things
		/// </summary>
		public enum Pages
		{
			/// <summary>
			/// The <see cref="Player"/>'s stats.
			/// </summary>
			STATS,
			/// <summary>
			/// The <see cref="Inventory"/>.
			/// </summary>
			ITEMS,
			/// <summary>
			/// The <c>Data</c> sub-page.
			/// </summary>
			DATA
		}

		#region SubPages
		/// <summary>
		/// STAT sub-menu pages which displays different things
		/// </summary>
		public enum StatsPages
		{
			/// <summary>
			/// The <see cref="Player"/>'s actives <see cref="Effect"/>s.
			/// </summary>
			Status,
			/// <summary>
			/// The <see cref="Player"/>'s SPECIAL <see cref="Attribute"/>s.
			/// </summary>
			SPECIAL,
			/// <summary>
			/// The <see cref="Player"/>'s skill <see cref="Attribute"/>s.
			/// </summary>
			Skills,
			/// <summary>
			/// The <see cref="Player"/>'s <see cref="Perk"/>s.
			/// </summary>
			Perks,
			/// <summary>
			/// The <see cref="Player"/>'s Reputations with other <see cref="Faction"/>s.
			/// </summary>
			General
		}

		/// <summary>
		/// DATA sub-menu pages which display different things
		/// </summary>
		public enum DataPages
		{
			/// <summary>
			/// The <see cref="Objects.Map"/> object with it's markers and legends.
			/// </summary>
			Map,
			/// <summary>
			/// The <see cref="List{Quest}"/> of active <see cref="Quest"/>s.
			/// </summary>
			Quests,
			/// <summary>
			/// The <c>Data</c> entries in the form of <c>*.txt</c> and <c>*.wav</c> files.
			/// </summary>
			Misc,
			/// <summary>
			/// The <see cref="Radio"/> object and it's <see cref="List{T}"/> of <c>Songs</c> file paths.
			/// </summary>
			Radio
		}
		#endregion
		#endregion
	}
}
