using Pip_Boy.Data_Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pip_Boy.Objects.PIP_Boy
{
	/// <summary>
	/// Handles all user interface display logic for the PIP-Boy.
	/// </summary>
	/// <remarks>
	/// Initializes a new instance of the <see cref="PipBoyUI"/> class.
	/// </remarks>
	/// <param name="pipBoy">Reference to the parent PipBoy instance.</param>
	/// <param name="color">The display color for the UI.</param>
	public class PipBoyUI(PipBoy pipBoy, ConsoleColor color)
	{
		private readonly PipBoy _pipBoy = pipBoy;

		/// <summary>
		/// The color of the <c>PIPBoy</c>'s text.
		/// </summary>
		public ConsoleColor Color { get; } = color;

		#region Display Methods
		/// <summary>
		/// Highlights a message in the <c>Console</c>.
		/// </summary>
		/// <param name="message">The message to highlight</param>
		/// <param name="newLine">Whether to start a new line or not.</param>
		public void Highlight(string message, bool newLine)
		{
			Console.BackgroundColor = Color;
			Console.ForegroundColor = ConsoleColor.Black;
			if (newLine)
			{
				Console.WriteLine(message);
			}
			else
			{
				Console.Write(message);
			}
			Console.ResetColor();
			Console.ForegroundColor = Color;
		}

		/// <summary>
		/// Displays an error message with sound.
		/// </summary>
		/// <param name="message">The error message to display</param>
		public void Error(string message)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Error.WriteLine(message);
			Console.Beep();
			Console.ForegroundColor = Color;
		}

		/// <summary>
		/// Displays a collection of items with a header.
		/// </summary>
		/// <param name="header">The title of the list</param>
		/// <param name="collection">The array of objects</param>
		/// <returns>A string representation of all items in the collection.</returns>
		public static string DisplayCollection<T>(string header, IEnumerable<T> collection) =>
			header + ':'
			+ Environment.NewLine
			+ string.Join(Environment.NewLine + '\t', collection);
		#endregion

		#region Menu Display
		/// <summary>
		/// Shows the content for the currently selected page.
		/// </summary>
		/// <param name="currentPage">The current main page</param>
		/// <param name="statPage">The current STAT sub-page</param>
		/// <param name="dataPage">The current DATA sub-page</param>
		/// <returns>The page content as a string</returns>
		public string ShowMenu(PipBoy.Pages currentPage, PipBoy.StatsPages statPage, PipBoy.DataPages dataPage) => currentPage switch
		{
			PipBoy.Pages.STATS => ShowStats(statPage),
			PipBoy.Pages.ITEMS => _pipBoy.Player.Inventory.ToString(),
			PipBoy.Pages.DATA => ShowData(dataPage),
			_ => throw new NotImplementedException()
		};

		/// <summary>
		/// Gets the submenu items for the current page.
		/// </summary>
		/// <param name="currentPage">The current main page</param>
		/// <returns>Array of submenu item names</returns>
		public static string[] GetSubMenu(PipBoy.Pages currentPage)
		{
			List<string> footer = [];
			Type enumType = currentPage switch
			{
				PipBoy.Pages.STATS => typeof(PipBoy.StatsPages),
				PipBoy.Pages.ITEMS => typeof(Inventory.ItemsPages),
				PipBoy.Pages.DATA => typeof(PipBoy.DataPages),
				_ => throw new NotImplementedException()
			};
			foreach (Enum subPage in Enum.GetValues(enumType))
			{
				footer.Add(subPage.ToString());
			}

			return [.. footer];
		}

		/// <summary>
		/// Shows and highlights the selected submenu for the page.
		/// </summary>
		/// <param name="subMenuItems">The submenu items of the current page</param>
		/// <param name="statPage">The current STAT sub-page</param>
		/// <param name="dataPage">The current DATA sub-page</param>
		public void ShowSubMenu(string[] subMenuItems, PipBoy.StatsPages statPage, PipBoy.DataPages dataPage)
		{
			foreach (string item in subMenuItems)
			{
				Console.Write('\t');
				if (item == statPage.ToString() || item == _pipBoy.Player.Inventory.itemPage.ToString() || item == dataPage.ToString())
				{
					Highlight(item, false);
				}
				else
				{
					Console.Write(item);
				}
				Console.Write('\t');
			}
			Console.WriteLine();
		}
		#endregion

		#region Page Content
		/// <summary>
		/// Shows the Stats Page content based on the selected sub-page.
		/// </summary>
		/// <param name="statPage">The current STAT sub-page</param>
		/// <returns>The stats content as a string</returns>
		public string ShowStats(PipBoy.StatsPages statPage) => statPage switch
		{
			PipBoy.StatsPages.Status => _pipBoy.Player.ShowStatus(),
			PipBoy.StatsPages.SPECIAL => DisplayCollection(nameof(_pipBoy.Player.SPECIAL), _pipBoy.Player.SPECIAL),
			PipBoy.StatsPages.Skills => DisplayCollection(nameof(_pipBoy.Player.Skills), _pipBoy.Player.Skills),
			PipBoy.StatsPages.Perks => DisplayCollection(nameof(_pipBoy.Player.Perks), _pipBoy.Player.Perks),
			PipBoy.StatsPages.General => ShowFactions(),
			_ => throw new NotImplementedException()
		};

		/// <summary>
		/// Shows all the factions and their statuses.
		/// </summary>
		/// <returns>A table of the factions</returns>
		public string ShowFactions()
		{
			StringBuilder stringBuilder = new();
			foreach (Faction faction in _pipBoy.Factions)
			{
				stringBuilder.AppendLine(faction.ToString());
			}
			return stringBuilder.ToString() + Environment.NewLine + _pipBoy.Factions[_pipBoy.FactionIndex].Description;
		}

		/// <summary>
		/// Shows the Data Page content based on the selected sub-page.
		/// </summary>
		/// <param name="dataPage">The current DATA sub-page</param>
		/// <returns>The data content as a string</returns>
		public string ShowData(PipBoy.DataPages dataPage) => dataPage switch
		{
			PipBoy.DataPages.Map => _pipBoy.Map.ToString(),
			PipBoy.DataPages.Quests => ShowQuests(),
			PipBoy.DataPages.Misc => ShowDataNotes(),
			PipBoy.DataPages.Radio => _pipBoy.Radio.ToString(),
			_ => throw new NotImplementedException()
		};

		/// <summary>
		/// Shows all active quests and their steps.
		/// </summary>
		/// <returns>A table of all quests and their steps</returns>
		public string ShowQuests()
		{
			StringBuilder stringBuilder = new();
			foreach (Quest quest in _pipBoy.Quests)
			{
				stringBuilder.AppendLine(quest.ToString());
			}
			return stringBuilder.ToString();
		}

		/// <summary>
		/// Shows all Data Notes.
		/// </summary>
		/// <returns>A table of all data notes and recordings</returns>
		public string ShowDataNotes()
		{
			StringBuilder stringBuilder = new();
			foreach (string data in Directory.GetFiles(_pipBoy.DataDirectory))
			{
				string extension = Path.GetExtension(data);
				string icon = extension switch
				{
					".txt" => "??",
					".wav" => "??",
					_ => "?",
				};

				stringBuilder.AppendLine(icon + ' ' + Path.GetFileNameWithoutExtension(data));
				stringBuilder.AppendLine(new string('-', 10));
				if (extension == ".txt")
				{
					stringBuilder.AppendLine(File.ReadAllText(data));
				}
			}
			return stringBuilder.ToString();
		}
		#endregion
	}
}
