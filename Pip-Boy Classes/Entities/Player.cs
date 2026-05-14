using Pip_Boy_Classes.Data_Types;
using Pip_Boy_Classes.Objects;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using static Pip_Boy_Classes.Data_Types.Effect;

namespace Pip_Boy_Classes.Entities
{
	/// <summary>
	/// This contains all player behavior.
	/// </summary>
	[DataContract]
	public class Player : Human
	{
		#region Radiation Stuff
		/// <summary>
		/// How much radiation the <see cref="Player"/> has, measured in RADs.
		/// </summary>
		[DataMember]
		public uint Rads { get; private set; } = 0u;

		/// <summary>
		/// The current level of radiation sickness the <see cref="Player"/> is experiencing, based on <see cref="Rads"/>.
		/// </summary>
		public RadiationSicknessLevels RadiationSicknessLevel => (RadiationSicknessLevels)Rads;

		/// <summary>
		/// The emoji icon that represents the current <see cref="RadiationSicknessLevel"/>.
		/// </summary>
		public string RadiationIcon => IconDeterminer.Determine(RadiationSicknessLevel);

		/// <summary>
		/// The <see cref="Effect"/>s to apply for each <see cref="RadiationSicknessLevel"/>.
		/// Each sub-array corresponds to a sickness level and contains the effects (e.g., stat penalties) applied at that level.
		/// </summary>
		[DataMember]
		public static readonly Effect[][] RadiationSicknessEffects = [
			[],
			[new(EffectTypes.Endurance, -1, 0)],
			[new(EffectTypes.Endurance, -2, 0), new(EffectTypes.Agility, -1, 0)],
			[new(EffectTypes.Endurance, -3, 0), new(EffectTypes.Agility, -2, 0), new(EffectTypes.Strength, -1, 0)],
			[new(EffectTypes.Endurance, -3, 0), new(EffectTypes.Agility, -2, 0), new(EffectTypes.Strength, -2, 0)],
			[new(EffectTypes.HitPoints, -128, 0)], // Instant Death
		];

		/// <summary>
		/// The percentage resistance to radiation, reducing the amount of RADs gained.
		/// </summary>
		[DataMember]
		public float RadiationResistance { get; private set; }
		#endregion

		#region Arrays
		/// <summary>
		/// <see cref="List{Perk}"/> of all active <see cref="Perk"/> on the <see cref="Player"/>.
		/// </summary>
		[DataMember]
		public List<Perk> Perks = [];
		#endregion

		#region Directories
		/// <summary>
		/// The directory from which files will be loaded and saved
		/// </summary>
		[DataMember]
		public readonly string activeDirectory;

		/// <summary>
		/// The directory from which <c>Perk</c>s will be loaded and saved
		/// </summary>
		[DataMember]
		public readonly string perksDirectory;

		/// <summary>
		/// The directory from which <see cref="Inventory"/> <see cref="Items.Item"/>s will be loaded and saved.
		/// </summary>
		[DataMember]
		public readonly string inventoryDirectory;
		#endregion

		#region Constructors
		/// <inheritdoc/>
		public Player() : base()
		{
			activeDirectory = string.Empty;
			perksDirectory = string.Empty;
			inventoryDirectory = string.Empty;
		}

		/// <summary>
		/// Player Creation using code
		/// </summary>
		/// <param name="name">The player's name</param>
		/// <param name="attributeValues">The special values</param>
		/// <param name="directory">The directory to load files from</param>
		public Player(string name, byte[] attributeValues, string directory) : base(name, 1)
		{
			activeDirectory = directory;
			// Use Path.Combine to build per-player subfolders to avoid accidental concatenation bugs
			perksDirectory = Path.Combine(activeDirectory, "Perks") + Path.DirectorySeparatorChar;
			inventoryDirectory = Path.Combine(activeDirectory, "Inventory") + Path.DirectorySeparatorChar;
			Inventory = new(inventoryDirectory, this);
			Icon = "🕹️";

			byte index = 0;
			for (int i = 0; i < SPECIAL.Length; i++)
			{
				SPECIAL[i].Value = attributeValues[index];
				index++;
			}

			LoadPlayerPerks();
		}
		#endregion

		#region Perk Stuff
		/// <summary>
		/// Serializes every <c>Perk</c> object in the <c>Perks</c> list to a file in the "Perks" folder.
		/// </summary>
		public void SavePlayerPerks()
		{
			foreach (Perk perk in Perks)
			{
				PipBoySerializer.ToFile(perksDirectory, perk);
			}
		}

		/// <summary>
		/// Deserializes every <c>Perk</c> file in the "Perks" folder list to an object in the <c>Perks</c>.
		/// </summary>
		public void LoadPlayerPerks()
		{
			string[] filePaths = Directory.GetFiles(perksDirectory, "*.xml");
			foreach (string filePath in filePaths)
			{
				Perks.Add(PipBoySerializer.FromFile<Perk>(filePath));
			}
		}

		/// <summary>
		/// Handles leveling up, and picking a <c>Perk</c>
		/// </summary>
		public void LevelUp()
		{
			Level++;
			if (Level % 2 == 0)
			{
				Perks.Add(new());
			}
		}
		#endregion

		/// <summary>
		/// Displays the Radiation statuses.
		/// </summary>
		/// <returns>The status screen</returns>
		public string RADScreen()
		{
			StringBuilder stringBuilder = new("RAD\t" + IconDeterminer.RadiationLogo);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("RADs: " + Rads);
			stringBuilder.AppendLine($"Radiation Resistance: {RadiationResistance:P}");
			stringBuilder.AppendLine("Radiation Sickness Level: " + RadiationSicknessLevel + '-' + RadiationIcon);
			stringBuilder.AppendLine("Radiation Effects:");
			int radiationEffectIndex = (int)(Rads / RadiationSicknessEffects.Length);
			foreach (Effect effect in RadiationSicknessEffects[radiationEffectIndex])
			{
				stringBuilder.AppendLine('\t' + effect.ToString());
			}
			return stringBuilder.ToString();
		}

		/// <summary>
		/// Represents the different levels of radiation sickness a player can experience, based on accumulated RADs.
		/// </summary>
		public enum RadiationSicknessLevels : uint
		{
			/// <summary>
			/// No radiation sickness (0-199 RADs). No negative effects.
			/// </summary>
			None = 0u,
			/// <summary>
			/// Minor radiation sickness (200-399 RADs). Small penalties to Endurance.
			/// </summary>
			Minor = 200u,
			/// <summary>
			/// Advanced radiation sickness (400-599 RADs). Greater penalties to Endurance and Agility.
			/// </summary>
			Advanced = 400u,
			/// <summary>
			/// Critical radiation sickness (600-799 RADs). Severe penalties to Endurance, Agility, and Strength.
			/// </summary>
			Critical = 600u,
			/// <summary>
			/// Deadly radiation sickness (800-999 RADs). Maximum stat penalties, near-fatal condition.
			/// </summary>
			Deadly = 800u,
			/// <summary>
			/// Fatal radiation poisoning (1000+ RADs). Instant death.
			/// </summary>
			Fatal = 1000u,
		}
	}
}
