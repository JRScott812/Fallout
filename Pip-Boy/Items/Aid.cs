using Pip_Boy.Data_Types;
using System;
using System.Runtime.Serialization;

namespace Pip_Boy.Items
{
	/// <summary>
	/// Represents an aid item, such as food, drink, or medicine, which can be used by the player to apply effects.
	/// </summary>
	[DataContract]
	public record Aid : Equipable
	{
		#region Variable(s)
		/// <summary>
		/// The specific type of aid this item represents (e.g., Food, Drink, Syringe).
		/// </summary>
		[DataMember]
		public readonly AidType TypeOfAid;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Aid"/> class with the specified name, weight, value, effects, and aid type.
		/// </summary>
		/// <param name="name">The name of the aid item.</param>
		/// <param name="weight">The weight of the aid item.</param>
		/// <param name="value">The value (in caps) of the aid item.</param>
		/// <param name="effects">The effects this aid item applies when used.</param>
		/// <param name="type">The type of aid (e.g., Food, Drink, Syringe).</param>
		public Aid(string name, float weight, ushort value, Effect[] effects, AidType type) : base(name, weight, value, effects)
		{
			TypeOfAid = type;
			Icon = IconDeterminer.Determine(type);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Aid"/> class for serialization.
		/// </summary>
		public Aid() : base()
		{
			TypeOfAid = 0;
			Icon = string.Empty;
		}
		#endregion

		#region Method(s)
		#endregion

		#region Enum(s)
		/// <summary>
		/// Enumerates the possible types of aid items.
		/// </summary>
		public enum AidType
		{
			/// <summary>
			/// Food item, such as meat, fruit, or packaged snacks. Restores health and may provide other effects.
			/// </summary>
			Food,
			/// <summary>
			/// Drink item, such as water, soda, or alcohol. Restores health, AP, or provides buffs/debuffs.
			/// </summary>
			Drink,
			/// <summary>
			/// Syringe-based medicine, such as Stimpaks or RadAway. Used for healing or removing radiation.
			/// </summary>
			Syringe,
			/// <summary>
			/// Pill-based medicine, such as Med-X or Mentats. Provides temporary buffs or resistances.
			/// </summary>
			Pill,
			/// <summary>
			/// Inhaled aid, such as Jet or Psycho. Provides temporary boosts, often with side effects.
			/// </summary>
			Inhale,
			/// <summary>
			/// Smoked aid, such as cigarettes or cigars. May provide minor buffs or roleplay effects.
			/// </summary>
			Smoke
		}
		#endregion
	}
}
