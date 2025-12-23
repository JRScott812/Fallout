using Pip_Boy.Entities;
using Pip_Boy.Items;
using Pip_Boy.Objects.PIP_Boy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pip_Boy.Objects
{
	/// <summary>
	/// Manages the player's inventory, including all item sub-lists (Weapons, Apparel, Aid, Misc, Ammo).
	/// Handles loading, saving, adding, and removing items, as well as carry weight calculations.
	/// </summary>
	public class Inventory
	{
		#region Lists
		/// <summary>
		/// All <see cref="Weapon"/>s in the <see cref="Inventory"/>.
		/// </summary>
		public List<Weapon> Weapons { get; private set; } = [];

		/// <summary>
		/// All <see cref="Apparel"/>s in the <see cref="Inventory"/>.
		/// </summary>
		public List<Apparel> Apparels { get; private set; } = [];

		/// <summary>
		/// All <see cref="Aid"/>s in the <see cref="Inventory"/>.
		/// </summary>
		public List<Aid> Aids { get; private set; } = [];

		/// <summary>
		/// All <see cref="Misc"/>s in the <see cref="Inventory"/>.
		/// </summary>
		public List<Misc> Miscs { get; private set; } = [];

		/// <summary>
		/// All <see cref="Ammo"/>s in the <see cref="Inventory"/>.
		/// </summary>
		public List<Ammo> Ammos { get; private set; } = [];
		#endregion

		/// <summary>
		/// The current <see cref="Item"/> sub-page, which determines which sub-list to display.
		/// </summary>
		public ItemsPages itemPage = ItemsPages.Weapons;

		#region Folders
		/// <summary>
		/// The sub-directories expected in the <see cref="InventoryFolderPath"/> directory.
		/// </summary>
		readonly string[] expectedSubDirectories = ["Weapon", "Apparel", "Aid", "Misc", "Ammo"];

		/// <summary>
		/// Directory which holds all sub-directories for <see cref="Inventory"/> <see cref="Item"/>s.
		/// </summary>
		public readonly string InventoryFolderPath;

		/// <summary>
		/// Directory which holds all serialized <see cref="Weapon"/> objects for the <see cref="Weapons"/> list.
		/// </summary>
		public readonly string WeaponFolderPath;

		/// <summary>
		/// Directory which holds all serialized <see cref="Apparel"/> objects for the <see cref="Apparels"/> list.
		/// </summary>
		public readonly string ApparelFolderPath;

		/// <summary>
		/// Directory which holds all serialized <see cref="Aid"/> objects for the <see cref="Aids"/> list.
		/// </summary>
		public readonly string AidFolderPath;

		/// <summary>
		/// Directory which holds all serialized <see cref="Misc"/> objects for the <see cref="Miscs"/> list.
		/// </summary>
		public readonly string MiscFolderPath;

		/// <summary>
		/// Directory which holds all serialized <see cref="Ammo"/> objects for the <see cref="Ammos"/> list.
		/// </summary>
		public readonly string AmmoFolderPath;
		#endregion

		/// <summary>
		/// The maximum weight the player can carry.
		/// </summary>
		public ushort MaxCarryWeight { get; private set; }

		/// <summary>
		/// The current weight of all items in the <see cref="Inventory"/>.
		/// </summary>
		public float CurrentCarryWeight => CalculateWeight();

		/// <summary>
		/// Indicates if the player is over-encumbered (i.e., carrying too much weight).
		/// </summary>
		public bool IsOverEncumbered => CurrentCarryWeight >= MaxCarryWeight;

		#region Constructors
		/// <summary>
		/// Initializes a new, empty <see cref="Inventory"/> for serialization.
		/// </summary>
		public Inventory()
		{
			InventoryFolderPath = string.Empty;
			WeaponFolderPath = string.Empty;
			ApparelFolderPath = string.Empty;
			AidFolderPath = string.Empty;
			MiscFolderPath = string.Empty;
			AmmoFolderPath = string.Empty;
		}

		/// <summary>
		/// Initializes a new <see cref="Inventory"/> by loading items from the specified folder and player.
		/// </summary>
		/// <param name="folderPath">The folder to load the items from.</param>
		/// <param name="player">The player, used to determine max carry weight.</param>
		/// <exception cref="DirectoryNotFoundException">Thrown if the folder does not exist.</exception>
		public Inventory(string folderPath, Player player)
		{
			if (!Directory.Exists(folderPath))
			{
				throw new DirectoryNotFoundException("Folder not found. " + folderPath);
			}

			// Set the folder paths
			if (!folderPath.EndsWith('\\'))
			{
				folderPath += '\\';
			}
			InventoryFolderPath = folderPath;
			WeaponFolderPath = InventoryFolderPath + "Weapon\\";
			ApparelFolderPath = InventoryFolderPath + "Apparel\\";
			AidFolderPath = InventoryFolderPath + "Aid\\";
			MiscFolderPath = InventoryFolderPath + "Misc\\";
			AmmoFolderPath = InventoryFolderPath + "Ammo\\";

			// Max carry weight is based on player's Strength SPECIAL stat
			MaxCarryWeight = (ushort)(150 + (player.SPECIAL[0].Value * 10));

			// Get the sub-directories of the Inventory directory
			string[] subDirectories = Directory.GetDirectories(folderPath);
			// Strip the containing directory info
			for (int index = 0; index < subDirectories.Length; index++)
			{
				subDirectories[index] = Path.GetFileName(subDirectories[index]);
			}

			foreach (string expectedSubDirectory in expectedSubDirectories)
			{
				if (subDirectories.Contains(expectedSubDirectory))
				{
					string currentFolder = folderPath + expectedSubDirectory + '\\';
					string[] filePaths = Directory.GetFiles(currentFolder, "*.xml");
					foreach (string filePath in filePaths)
					{
						switch (expectedSubDirectory)
						{
							case "Weapon":
								Weapons.Add(PipBoySerializer.FromFile<Weapon>(filePath));
								break;
							case "Apparel":
								if (PipBoySerializer.GetTypeFromXML(filePath) == typeof(HeadPiece))
									Apparels.Add(PipBoySerializer.FromFile<HeadPiece>(filePath));
								else if (PipBoySerializer.GetTypeFromXML(filePath) == typeof(TorsoPiece))
									Apparels.Add(PipBoySerializer.FromFile<TorsoPiece>(filePath));
								break;
							case "Aid":
								Aids.Add(PipBoySerializer.FromFile<Aid>(filePath));
								break;
							case "Misc":
								Miscs.Add(PipBoySerializer.FromFile<Misc>(filePath));
								break;
							case "Ammo":
								Ammos.Add(PipBoySerializer.FromFile<Ammo>(filePath));
								break;
						}
					}
				}
			}
		}
		#endregion

		/// <summary>
		/// Calculates the total weight of all items in the <see cref="Inventory"/>.
		/// </summary>
		/// <returns>The total weight of all items.</returns>
		private float CalculateWeight() => Weapons.Sum(x => x.Weight) + Apparels.Sum(x => x.Weight) + Aids.Sum(x => x.Weight) + Miscs.Sum(x => x.Weight) + Ammos.Sum(x => x.Weight);

		/// <summary>
		/// Writes all items in the <see cref="Inventory"/> sub-lists to files in the correct directories.
		/// </summary>
		public void Save()
		{
			Weapons.ForEach(x => PipBoySerializer.ToFile(WeaponFolderPath, x));
			Apparels.ForEach(x => PipBoySerializer.ToFile(ApparelFolderPath, x));
			Aids.ForEach(x => PipBoySerializer.ToFile(AidFolderPath, x));
			Apparels.ForEach(x => PipBoySerializer.ToFile(ApparelFolderPath, x));
			Ammos.ForEach(x => PipBoySerializer.ToFile(AmmoFolderPath, x));
		}

		#region Inventory Management
		/// <summary>
		/// Clears all <see cref="Inventory"/> sub-lists, removing all items.
		/// </summary>
		public void Clear()
		{
			Weapons.Clear();
			Apparels.Clear();
			Aids.Clear();
			Miscs.Clear();
			Ammos.Clear();
		}

		/// <summary>
		/// Adds a generic <see cref="Item"/> to its correct sub-list and sorts the list by name.
		/// </summary>
		/// <param name="item">The <see cref="Item"/> to add.</param>
		public void Add(Item item)
		{
			switch (item)
			{
				case Weapon weapon:
					Weapons.Add(weapon);
					Weapons.Sort((x, y) => string.Compare(x.Name, y.Name));
					break;
				case Apparel apparel:
					Apparels.Add(apparel);
					Apparels.Sort((x, y) => string.Compare(x.Name, y.Name));
					break;
				case Aid aid:
					Aids.Add(aid);
					Aids.Sort((x, y) => string.Compare(x.Name, y.Name));
					break;
				case Misc misc:
					Miscs.Add(misc);
					Miscs.Sort((x, y) => string.Compare(x.Name, y.Name));
					break;
				case Ammo ammo:
					Ammos.Add(ammo);
					Ammos.Sort((x, y) => string.Compare(x.Name, y.Name));
					break;
			}
		}

		/// <summary>
		/// Removes an <see cref="Item"/> from its sub-list and sorts the list by name.
		/// </summary>
		/// <param name="item">The <see cref="Item"/> to remove.</param>
		public void Drop(Item item)
		{
			switch (item)
			{
				case Weapon weapon:
					Weapons.Remove(weapon);
					Weapons.Sort((x, y) => string.Compare(x.Name, y.Name));
					break;
				case Apparel apparel:
					Apparels.Remove(apparel);
					Apparels.Sort((x, y) => string.Compare(x.Name, y.Name));
					break;
				case Aid aid:
					Aids.Remove(aid);
					Weapons.Sort((x, y) => string.Compare(x.Name, y.Name));
					break;
				case Misc misc:
					Miscs.Remove(misc);
					Miscs.Sort((x, y) => string.Compare(x.Name, y.Name));
					break;
				case Ammo ammo:
					Ammos.Remove(ammo);
					Ammos.Sort((x, y) => string.Compare(x.Name, y.Name));
					break;
			}
		}

		/// <summary>
		/// Counts how many occurrences of an <see cref="Item"/> are in a <see cref="List{Item}"/>.
		/// </summary>
		/// <param name="list">The <see cref="List{Item}"/> to search through.</param>
		/// <param name="item">The <see cref="Item"/> to look for.</param>
		/// <returns>The number of occurrences of the item in the list.</returns>
		public static int CountItem(List<Item> list, Item item)
		{
			if (list.Contains(item))
			{
				int count = 1;
				for (int index = list.FindIndex(f => f.Name == item.Name) + 1; index < list.Count; index++)
				{
					if (list[index].Name == item.Name)
					{
						count++;
					}
					// Ideally the list should be sorted alphabetically by name, 
					// so there is no need to keep checking once the name isn't the same.
					else
					{
						return count;
					}
				}
				return count;
			}
			else
			{
				return 0;
			}
		}
		#endregion

		/// <summary>
		/// Returns a string representation of the inventory, including carry weight and the current item page's items.
		/// </summary>
		/// <returns>
		/// A table of every <see cref="itemPage"/> <see cref="Item"/>'s <see cref="Item.ToString()"/>.
		/// </returns>
		public override string ToString() =>
			$"{CurrentCarryWeight}/{MaxCarryWeight} -- Over Encumbered?: {IsOverEncumbered}"
			+ Environment.NewLine
			+ new string('-', 15)
			+ Environment.NewLine
			+ itemPage switch
			{
				ItemsPages.Weapons => string.Join(Environment.NewLine, Weapons),
				ItemsPages.Apparel => string.Join(Environment.NewLine, Apparels),
				ItemsPages.Aid => string.Join(Environment.NewLine, Aids),
				ItemsPages.Ammo => string.Join(Environment.NewLine, Ammos),
				ItemsPages.Misc => string.Join(Environment.NewLine, Miscs),
				_ => throw new InvalidDataException("Invalid ItemsPage!"),
			};

		/// <summary>
		/// Enumerates the possible item sub-menu pages for the inventory.
		/// </summary>
		public enum ItemsPages
		{
			/// <summary>
			/// The weapons page, displaying all <see cref="Weapon"/>s.
			/// </summary>
			Weapons,
			/// <summary>
			/// The apparel page, displaying all <see cref="Apparel"/>s.
			/// </summary>
			Apparel,
			/// <summary>
			/// The aid page, displaying all <see cref="Aid"/>s.
			/// </summary>
			Aid,
			/// <summary>
			/// The ammo page, displaying all <see cref="Ammo"/>s.
			/// </summary>
			Ammo,
			/// <summary>
			/// The misc page, displaying all <see cref="Misc"/> items.
			/// </summary>
			Misc
		}
	}
}