using Pip_Boy.Data_Types;
using Pip_Boy.Objects;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Pip_Boy.Items
{
	/// <summary>
	/// Can be used to attack <see cref="Entities.Entity"/>s.
	/// </summary>
	[DataContract]
	public record Weapon : Equipable
	{
		#region Variable(s)
		/// <summary>
		/// The original damage, unaffected by the <see cref="Weapon"/>'s <see cref="Equipable.Condition"/>.
		/// </summary>
		[DataMember]
		private readonly byte originalDamage;

		/// <summary>
		/// The varying damage which is just: <code><see cref="originalDamage"/> * <see cref="Equipable.Condition"/></code>
		/// </summary>
		public byte Damage => (byte)(originalDamage * Condition);

		/// <summary>
		/// How many times the weapon can be used per minute.
		/// </summary>
		[DataMember]
		public ushort RateOfFire;

		/// <summary>
		/// Average Damage per Second based on <see cref="Damage"/> per shot and <see cref="RateOfFire"/>.
		/// </summary>
		public byte DPS => (byte)(RateOfFire / 60 * Damage);

		/// <summary>
		/// The required 'Strength' level in the SPECIAL attributes to effectively use the <see cref="Weapon"/>.
		/// </summary>
		[DataMember]
		public byte StrengthRequirement;

		/// <summary>
		/// The required skill level in the skill attributes to effectively use the <see cref="Weapon"/>.
		/// </summary>
		[DataMember]
		public byte SkillRequirement;

		/// <summary>
		/// What kind of <see cref="Weapon"/> is it, determining the type of damage dealt.
		/// </summary>
		[DataMember]
		public readonly WeaponType TypeOfWeapon;

		/// <summary>
		/// All equipped modifications on the <see cref="Weapon"/>.
		/// </summary>
		[DataMember]
		public List<string> Modifications = [];
		#endregion

		#region Constructors
		/// <inheritdoc/>
		public Weapon(string name, float weight, ushort value, Effect[] effects, WeaponType weaponType, byte strengthRequirement, byte skillRequirement, byte damage, ushort rateOfFire) : base(name, weight, value, effects)
		{
			TypeOfWeapon = weaponType;
			StrengthRequirement = strengthRequirement;
			SkillRequirement = skillRequirement;
			originalDamage = damage;
			RateOfFire = rateOfFire;

			Icon = IconDeterminer.Determine(weaponType);
		}

		/// <inheritdoc/>
		public Weapon() : base()
		{
			TypeOfWeapon = 0;
			StrengthRequirement = 0;
			SkillRequirement = 0;
			originalDamage = 0;
			RateOfFire = 0;

			Icon = string.Empty;
		}
		#endregion

		#region Method(s)
		#endregion

		#region Enum(s)
		/// <summary>
		/// The type of weapon, which determines what <see cref="Ammo"/> can be used.
		/// </summary>
		public enum WeaponType
		{
			/// <summary>
			/// Guns are the most common ranged weapons of the Mojave Wasteland. They are numerous and varied and ammunition is relatively easy to acquire.
			/// </summary>
			Gun,

			/// <summary>
			/// Explosive weapons are best used when dealing with crowds or in situations where precision is not a high priority.
			/// </summary>
			Explosive,

			/// <summary>
			/// Energy weapons are less common and varied than guns, but have a small number of ammunition types and can be quite potent.
			/// </summary>
			Energy,

			/// <summary>
			/// Most Melee Weapons are completely silent, making them excellent stealth weapons.
			/// </summary>
			Melee,

			/// <summary>
			/// Fisticuffs.
			/// </summary>
			Unarmed
		}
		#endregion

		#region Override Functions
		/// <returns><see cref="Equipable.ToString()"/> is there are no <see cref="Modifications"/>.  If there are, then add them to the string.</returns>
		public override string ToString() => Modifications.Count == 0 ? base.ToString() : PipBoy.DisplayCollection(nameof(Modifications), Modifications);
		#endregion
	}
}
