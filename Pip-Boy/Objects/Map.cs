using Pip_Boy.Data_Types;
using Pip_Boy.Entities;
using Pip_Boy.Objects.PIP_Boy;
using System;
using System.IO;
using System.Numerics;
using System.Text;

namespace Pip_Boy.Objects
{
	/// <summary>
	/// Contains locations of places of interest.
	/// </summary>
	public class Map
	{
		/// <summary>
		/// Private array containing all locations.
		/// </summary>
		private readonly Location[] Locations;

		/// <summary>
		/// The 2D <see cref="Location"/> array, which is the visual representation of the <see cref="Map"/>.
		/// </summary>
		public readonly Location?[,] Grid;

		/// <summary>
		/// The location of the <see cref="Player"/>
		/// </summary>
		Vector2 PlayerLocation = new();

		#region Constructor
		/// <summary>
		/// Creates a <see cref="Map"/> with the given dimensions
		/// </summary>
		/// <param name="height">The height of the map</param>
		/// <param name="width">The width of the map</param>
		/// <param name="mapLocationsFolder">The folder to load the locations from</param>
		public Map(byte height, byte width, string mapLocationsFolder)
		{
			Locations = LoadLocations(mapLocationsFolder);
			Grid = GenerateMap(width, height);
		}
		#endregion

		/// <summary>
		/// Create a map with the given parameters
		/// </summary>
		/// <param name="height">The height of the map</param>
		/// <param name="width">The width of the map</param>
		/// <returns></returns>
		public Location?[,] GenerateMap(byte height, byte width)
		{
			Location?[,] tempMap = new Location?[width, height];

			// Place locations on the map
			foreach (Location location in Locations)
			{
				tempMap[(int)location.Position.Y, (int)location.Position.X] = location;
			}
			return tempMap;
		}

		/// <summary>
		/// Create a map with the given parameters
		/// </summary>
		/// <param name="folder">Folder to load the <see cref="Location"/>s from</param>
		/// <returns></returns>
		public static Location[] LoadLocations(string folder)
		{
			string[] filePaths = Directory.GetFiles(folder, "*.xml");
			Location[] tempLocations = new Location[filePaths.Length];
			for (int i = 0; i < filePaths.Length; i++)
			{
				string filePath = filePaths[i];
				tempLocations[i] = PipBoySerializer.FromFile<Location>(filePath);
			}
			return tempLocations;
		}

		/// <summary>
		/// Moves the player icon on the map to the current X and Y coordinates
		/// </summary>
		/// <param name="up">If the player moves up/down</param>
		/// <param name="right">If the player moves right/left</param>
		/// <param name="player">The Player object to get the <see cref="Entity.Location"/> value from</param>
		public void MovePlayer(bool? up, bool? right, Player player)
		{
			PlayerLocation = player.Location;

			switch (up)
			{
				case true when player.Location.Y > 0:
					player.Location.Y--;
					break;
				case false when player.Location.Y < Grid.GetLength(0):
					player.Location.Y++;
					break;
			}

			switch (right)
			{
				case true when player.Location.X < Grid.GetLength(1):
					player.Location.X++;
					break;
				case false when player.Location.X > 0:
					player.Location.X--;
					break;
			}
		}

		/// <summary>
		/// Shows the map
		/// </summary>
		/// <returns>The 2D array as a string</returns>
		public override string ToString()
		{
			StringBuilder stringBuilder = new();
			for (int row = 0; row < Grid.GetLength(0); row++)
			{
				for (int col = 0; col < Grid.GetLength(1); col++)
				{
					if (PlayerLocation == new Vector2(col, row))
					{
						stringBuilder.Append("🕹️");
					}
					else
					{
						Location? location = Grid[row, col];
						stringBuilder.Append(location is not null ? location.Icon : ' ');
					}
				}
				stringBuilder.AppendLine();
			}
			stringBuilder.AppendLine(string.Join(Environment.NewLine, (object[])Locations));
			return stringBuilder.ToString();
		}
	}
}
