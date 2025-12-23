using Pip_Boy.Data_Types;
using System;
using System.Runtime.Serialization;

namespace Pip_Boy.Items
{
	/// <summary>
	/// Represents ammunition required by non-<see cref="Weapon.WeaponType.Melee"/> and non-<see cref="Weapon.WeaponType.Melee"/> <see cref="Weapon"/>s.
	/// Stores the ammo type and any modifications that affect weapon performance.
	/// </summary>
	[DataContract]
	public record Ammo : Equipable
	{
		#region Variable(s)
		/// <summary>
		/// The type of ammo required for the <see cref="Weapon"/> (e.g., Bullet, Bomb, EnergyCell).
		/// </summary>
		[DataMember]
		public readonly AmmoType TypeOfAmmo;

		/// <summary>
		/// The modification applied to the <see cref="Ammo"/>.
		/// Modifications can change damage, damage type, or add special effects.
		/// </summary>
		[DataMember]
		public readonly AmmoModification Modification;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Ammo"/> class with the specified name, value, effects, ammo type, and modification.
		/// </summary>
		/// <param name="name">The name of the ammo item.</param>
		/// <param name="value">The value (in caps) of the ammo item.</param>
		/// <param name="effects">The effects this ammo applies when used.</param>
		/// <param name="ammoType">The type of ammo (e.g., Bullet, Bomb, EnergyCell).</param>
		/// <param name="ammoModification">The modification applied to the ammo (e.g., HollowPoint, Explosive).</param>
		public Ammo(string name, ushort value, Effect[] effects, AmmoType ammoType, AmmoModification ammoModification) : base(name, 0, value, effects)
		{
			TypeOfAmmo = ammoType;
			Modification = ammoModification;

			Icon = IconDeterminer.Determine(ammoType);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Ammo"/> class for serialization.
		/// </summary>
		public Ammo() : base()
		{
			TypeOfAmmo = 0;
			Modification = 0;

			Icon = string.Empty;
		}
		#endregion

		#region Method(s)
		#endregion

		#region Enum(s)
		/// <summary>
		/// The type of ammo, which determines which <see cref="Weapon"/>s can use it.
		/// </summary>
		public enum AmmoType
		{
			/// <summary>
			/// Standard ballistic ammunition, such as bullets for pistols and rifles.
			/// </summary>
			Bullet,
			/// <summary>
			/// Explosive ammunition, such as grenades, mines, or rockets.
			/// </summary>
			Bomb,
			/// <summary>
			/// Energy-based ammunition, such as microfusion cells or electron charge packs.
			/// </summary>
			EnergyCell,
		}

		/// <summary>
		/// The modifications on the <see cref="Ammo"/> object, which change damage and <see cref="Effect"/>s.
		/// </summary>
		public enum AmmoModification
		{
			/// <summary>
			/// Standard, unmodified ammunition.
			/// </summary>
			Standard,
			/// <summary>
			/// Hollow Point: Increased damage to unarmored targets, reduced against armor.
			/// </summary>
			HollowPoint,
			/// <summary>
			/// Armor Piercing: Reduced damage but ignores a portion of target's armor.
			/// </summary>
			ArmorPiercing,
			/// <summary>
			/// Hand Load: Custom, hand-loaded rounds with improved stats.
			/// </summary>
			HandLoad,
			/// <summary>
			/// Special: Unique or rare modification with special effects.
			/// </summary>
			Special,
			/// <summary>
			/// Surplus: Cheap, unreliable rounds with increased wear or misfire chance.
			/// </summary>
			Surplus,
			/// <summary>
			/// Explosive: Rounds that explode on impact, dealing area damage.
			/// </summary>
			Explosive,
			/// <summary>
			/// Incendiary: Rounds that ignite targets, causing fire damage over time.
			/// </summary>
			Incendiary
		}
		#endregion

		#region Override Functions
		/// <summary>
		/// Returns a string representation of the ammo, including its type and modification.
		/// </summary>
		/// <returns>
		/// The base item string, followed by ammo type and modification details.
		/// </returns>
		public override string ToString() => base.ToString() + $"{Environment.NewLine}\t\tAmmo Type: {TypeOfAmmo}{Environment.NewLine}\t\tAmmo Modification: {Modification}{IconDeterminer.Determine(Modification)}";
		#endregion
	}
}
