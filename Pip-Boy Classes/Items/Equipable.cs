using Pip_Boy_Classes.Data_Types;
using Pip_Boy_Classes.Entities;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Pip_Boy_Classes.Items
{
	/// <summary>
	/// Defines behaviors for <see cref="Equipable"/> sub-classes, such as <see cref="Weapon"/>, <see cref="HeadPiece"/>, <see cref="TorsoPiece"/> and <see cref="Ammo"/>.
	/// </summary>
	[DataContract]
	public abstract record Equipable : Item
	{
		#region Variable(s)
		/// <summary>
		/// The original value of the <see cref="Equipable"/>, unaffected by <see cref="Condition"/>.
		/// </summary>
		[DataMember]
		private readonly ushort originalValue;

		/// <summary>
		/// The percentage of the <see cref="Equipable"/> used to determine <see cref="Weapon.Damage"/>, <see cref="Apparel.DamageThreshold"/> and <see cref="Item.Value"/>.
		/// </summary>
		[DataMember]
		public float Condition = 1f;

		/// <summary>
		/// If the <see cref="Equipable"/> is equipped to an <see cref="Entity"/>
		/// </summary>
		[DataMember]
		public bool IsEquipped = false;

		/// <summary>
		/// The effects that the <see cref="Equipable"/> will apply when <see cref="Equip(Entity)"/>'d to an <see cref="Entity"/>.
		/// </summary>
		[DataMember]
		public readonly List<Effect> Effects;
		#endregion

		#region Constructors
		/// <inheritdoc/>
		public Equipable(string name, float weight, ushort value, Effect[] effects) : base(name, weight, value)
		{
			originalValue = value;
			Effects = [.. effects];
		}

		/// <inheritdoc/>
		public Equipable() : base()
		{
			originalValue = 0;
			Effects = [];
		}
		#endregion

		#region Method(s)
		/// <summary>
		/// Appends the common equipable details to a string builder.
		/// </summary>
		/// <param name="builder">The builder to append to.</param>
		protected void AppendEquipableDetails(StringBuilder builder)
		{
			builder.AppendLine($"\tCondition: {Condition:0.00}");
			builder.AppendLine($"\tEquipped: {(IsEquipped ? "Yes" : "No")}");

			if (Effects.Count == 0)
			{
				return;
			}

			builder.AppendLine("\tEffects:");
			foreach (Effect effect in Effects)
			{
				builder.AppendLine($"\t\t{effect}");
			}
		}

		/// <summary>
		/// Equips this <see cref="Equipable"/> to an <see cref="Entity"/>.
		/// </summary>
		/// <param name="entity">The <see cref="Entity"/> to equip the <see cref="Equipable"/> to.</param>
		public void Equip(Entity entity)
		{
			IsEquipped = true;
			entity.Effects.AddRange(Effects);
			entity.ApplyEffects();
		}

		/// <summary>
		/// Unequips this <see cref="Equipable"/> from an <see cref="Entity"/>.
		/// </summary>
		/// <param name="entity">The <see cref="Entity"/> to unequip the <see cref="Equipable"/> from.</param>
		public void Unequip(Entity entity)
		{
			IsEquipped = false;
			foreach (Effect effect in entity.Effects)
			{
				Effects.Remove(effect);
			}
		}

		/// <summary>
		/// Update the value of the <see cref="Equipable"/> based on the <see cref="Condition"/>.
		/// </summary>
		public void UpdateValue()
		{
			Value = (ushort)(originalValue * Condition);
		}
		#endregion

		#region Enum(s)
		#endregion

		#region Override Functions
		/// <returns>The item output, followed by equipable details.</returns>
		public override string ToString()
		{
			StringBuilder result = new(base.ToString());
			result.AppendLine();
			AppendEquipableDetails(result);
			return result.ToString().TrimEnd();
		}
		#endregion
	}
}
