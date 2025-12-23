using Pip_Boy.Data_Types;
using Pip_Boy.Items;
using Pip_Boy.Objects;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;

namespace Pip_Boy.Entities
{
	/// <summary>
	/// Generic type which other <see cref="Entity"/> sub-classes will inherit from
	/// </summary>
	[DataContract]
	public abstract class Entity
	{
		#region Arrays
		/// <summary>
		/// This holds all objects belonging to the <see cref="Entity"/>.
		/// </summary>
		[DataMember]
		public Inventory Inventory;

		/// <summary>
		/// The SPECIAL attributes, which effects player stats.
		/// The order will not change.
		/// </summary>
		[DataMember]
		public Data_Types.Attribute[] SPECIAL =
		[
			new(Data_Types.Attribute.AttributeName.Strength, 1),
			new(Data_Types.Attribute.AttributeName.Perception, 1),
			new(Data_Types.Attribute.AttributeName.Endurance, 1),
			new(Data_Types.Attribute.AttributeName.Charisma, 1),
			new(Data_Types.Attribute.AttributeName.Intelligence, 1),
			new(Data_Types.Attribute.AttributeName.Agility, 1),
			new(Data_Types.Attribute.AttributeName.Luck, 1)
		];

		/// <summary>
		/// The Skills, which effects player stats.
		/// The order will not change.
		/// </summary>
		[DataMember]
		public Data_Types.Attribute[] Skills =
		[
			new(Data_Types.Attribute.AttributeName.Barter, 10),
			new(Data_Types.Attribute.AttributeName.EnergyWeapons, 10),
			new(Data_Types.Attribute.AttributeName.Explosives, 10),
			new(Data_Types.Attribute.AttributeName.Gun, 10),
			new(Data_Types.Attribute.AttributeName.Lockpick, 10),
			new(Data_Types.Attribute.AttributeName.Medicine, 10),
			new(Data_Types.Attribute.AttributeName.MeleeWeapons, 10),
			new(Data_Types.Attribute.AttributeName.Repair, 10),
			new(Data_Types.Attribute.AttributeName.Science, 10),
			new(Data_Types.Attribute.AttributeName.Sneak, 10),
			new(Data_Types.Attribute.AttributeName.Speech, 10),
			new(Data_Types.Attribute.AttributeName.Survival, 10),
			new(Data_Types.Attribute.AttributeName.Unarmed, 10)
		];

		/// <summary>
		/// The <see cref="Entity"/>s limbs, which can be targeted.
		/// </summary>
		[DataMember]
		public Limb[] Limbs;

		/// <summary>
		/// All <see cref="Effect"/>s that are active on the <see cref="Entity"/>.
		/// </summary>
		[DataMember]
		public List<Effect> Effects = [];
		#endregion

		#region Entity Info
		/// <summary>
		/// The name of the <see cref="Entity"/>.
		/// </summary>
		[DataMember]
		public readonly string Name;

		/// <summary>
		/// The <see cref="Entity"/>'s level, which determines attributes.
		/// </summary>
		[DataMember]
		public byte Level;

		/// <summary>
		/// The gender of the <see cref="Humanoid"/>, <c>false</c> is Male <c>true</c> is female (since <c>bool</c> defaults to <c>false</c>
		/// </summary>
		[DataMember]
		public readonly bool Gender;

		/// <summary>
		/// The maximum health the <see cref="Entity"/> can have.
		/// </summary>
		[DataMember]
		public ushort MaxHealth { get; private set; }

		/// <summary>
		/// The current health the <see cref="Entity"/> has.
		/// </summary>
		[DataMember]
		public int CurrentHealth { get; private set; }

		/// <summary>
		/// What percent of health the <see cref="Entity"/> has.
		/// </summary>
		public float HealthPercentage => CurrentHealth / MaxHealth;

		/// <summary>
		/// The resistance to physical damage.
		/// </summary>
		[DataMember]
		public byte DamageResistance { get; private set; }

		/// <summary>
		/// The maximum number of Action Points (AP) the <see cref="Entity"/> has.
		/// </summary>
		[DataMember]
		public byte MaxActionPoints { get; private set; }

		/// <summary>
		/// The current amount of Action Points (AP) the <see cref="Entity"/> has.
		/// </summary>
		[DataMember]
		public byte ActionPoints { get; private set; }

		/// <summary>
		/// An emoji representing the <see cref="Entity"/>.
		/// </summary>
		[DataMember]
		public string Icon;

		/// <summary>
		/// Where the <see cref="Entity"/> is on the <see cref="Map"/>/area.
		/// </summary>
		[DataMember]
		public Vector2 Location;
		#endregion

		#region Constructor
		/// <summary>
		/// Empty constructor for Serialization.
		/// </summary>
		public Entity()
		{
			Name = string.Empty;
			Inventory = new();
			Icon = string.Empty;
			Limbs = [];
		}

		/// <summary>
		/// Construction based on level.
		/// </summary>
		public Entity(string name, byte level)
		{
			Name = name;
			Inventory = new();
			Icon = string.Empty;
			Level = level;
			Limbs = [];

			// Set attribute to random values, based on the level
			Random random = new();

			for (int i = 0; i < SPECIAL.Length; i++)
			{
				SPECIAL[i].Value = (byte)random.Next(1, 10);
			}

			for (int i = 0; i < Skills.Length; i++)
			{
				Skills[i].Value = (byte)random.Next(10, 100);
			}

			MaxHealth = (ushort)random.Next(100, 250);
			CurrentHealth = MaxHealth;

			MaxActionPoints = (byte)random.Next(5, 35);
			ActionPoints = MaxActionPoints;
		}
		#endregion

		#region EquippedItems
		/// <summary>
		/// The equipped <see cref="HeadPiece"/>, which can be null
		/// </summary>
		public HeadPiece? headPiece;

		/// <summary>
		/// The equipped <see cref="TorsoPiece"/>, which can be null
		/// </summary>
		public TorsoPiece? torsoPiece;

		/// <summary>
		/// The equipped <see cref="Weapon"/>, which can be null
		/// </summary>
		public Weapon? weapon;

		/// <summary>
		/// The equipped <see cref="Aid"/>, which can be null.  The item is "used" once equipped.
		/// </summary>
		public Aid? aid;

		/// <summary>
		/// The equipped <see cref="Ammo"/>, for the <see cref="weapon"/> which can be null, if the <see cref="weapon"/> is <see cref="Weapon.WeaponType.Melee"/> or <see cref="Weapon.WeaponType.Unarmed"/>
		/// </summary>
		public Ammo? ammo;
		#endregion

		#region Items
		/// <summary>
		/// Equips the <see cref="Equipable"/> to the correct spot, depending on it's type
		/// </summary>
		/// <param name="item">The item to equip.</param>
		public void Equip(Equipable item)
		{
			switch (item)
			{
				case HeadPiece headPieceItem:
					headPiece = headPieceItem;
					break;
				case TorsoPiece torsoPieceItem:
					torsoPiece = torsoPieceItem;
					break;
				case Weapon weaponItem:
					weapon = weaponItem;
					break;
				case Ammo ammoItem:
					ammo = ammoItem;
					break;
			}
			item.Equip(this);
		}

		/// <summary>
		/// Unequips the <see cref="Equipable"/> from the correct spot, depending on it's type
		/// </summary>
		/// <param name="item">The item to unequip.</param>
		public void Unequip(Equipable item)
		{
			if (item is not null)
			{
				item.Unequip(this);
				switch (item)
				{
					case HeadPiece:
						headPiece = null;
						break;
					case TorsoPiece:
						torsoPiece = null;
						break;
					case Weapon:
						weapon = null;
						break;
					case Ammo:
						ammo = null;
						break;
				}
			}
		}
		#endregion

		#region Effects
		/// <summary>
		/// Applies all <see cref="Effect"/>s from <see cref="Effects"/> to the <see cref="Entity"/>.
		/// </summary>
		public void ApplyEffects()
		{
			ResetEffects();
			foreach (Effect effect in Effects)
			{
				for (int i = 0; i < SPECIAL.Length; i++)
				{
					Data_Types.Attribute attribute = SPECIAL[i];
					if (effect.Effector.ToString() == attribute.Name.ToString())
					{
						if (attribute.Value + effect.Value >= 1)
						{
							attribute.Value = (byte)(attribute.Value + effect.Value);
						}
					}
				}
				for (int i = 0; i < Skills.Length; i++)
				{
					Data_Types.Attribute skill = Skills[i];
					if (effect.Effector.ToString() == skill.Name.ToString())
					{
						if (skill.Value + effect.Value >= 10)
						{
							skill.Value = (byte)(skill.Value + effect.Value);
						}
					}
				}
			}
		}

		/// <summary>
		/// Clears all <see cref="Effect"/>s from <see cref="Effects"/>.
		/// </summary>
		public void ResetEffects()
		{
			Effects.Clear();
		}
		#endregion

		#region Show Entity Info
		/// <summary>
		/// Shows the <see cref="Entity"/>'s current status
		/// </summary>
		/// <returns>A table of the <see cref="Entity"/>'s name, level and current health</returns>
		public string ShowStatus()
		{
			StringBuilder stringBuilder = new("Status:");
			stringBuilder.AppendLine("Name:\t" + Name + Icon);
			stringBuilder.AppendLine("Level:\t" + Level);
			stringBuilder.AppendLine("Health:\t" + CurrentHealth + '/' + MaxHealth);
			stringBuilder.AppendLine("Action Points:\t" + ActionPoints + '/' + MaxActionPoints);
			return stringBuilder.ToString();
		}

		/// <returns>The limbs and their percentages</returns>
		public virtual string ShowLimbs() => PipBoy.DisplayCollection(nameof(Limbs), Limbs);

		/// <summary>
		/// Show 1-line preview of the <see cref="Entity"/>
		/// </summary>
		/// <returns><see cref="Name"/>, <see cref="Icon"/>, <see cref="Gender"/>, <see cref="Level"/> and <see cref="HealthPercentage"/></returns>
		public override string ToString() => $"{Name}{Icon}{IconDeterminer.Determine(Gender)}-{Level}: {HealthPercentage:P}";
		#endregion
	}
}
