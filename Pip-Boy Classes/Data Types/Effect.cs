using System.Runtime.Serialization;

namespace Pip_Boy_Classes.Data_Types
{
	/// <summary>
	/// Represents an effect that changes a <see cref="Pip_Boy_Classes.Entities.Player"/>'s attributes, skills, or stats for a certain duration.
	/// </summary>
	[DataContract]
	public partial class Effect
	{
		/// <summary>
		/// The attribute, skill, or stat that this <see cref="Effect"/> will change.
		/// </summary>
		[DataMember]
		public readonly EffectTypes Effector;

		/// <summary>
		/// The amount by which the effector will be changed (positive or negative).
		/// </summary>
		[DataMember]
		public readonly sbyte Value;

		/// <summary>
		/// The icon representing this effect.
		/// </summary>
		[DataMember]
		public readonly string Icon;

		/// <summary>
		/// The total duration (in turns or seconds) that this effect will last. -1 means the effect is permanent while equipped.
		/// </summary>
		[DataMember]
		public readonly short Duration;

		/// <summary>
		/// The remaining time (in turns or seconds) for which this effect is still active.
		/// </summary>
		[DataMember]
		public short TimeLeft;

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Effect"/> class for serialization.
		/// </summary>
		public Effect()
		{
			Effector = EffectTypes.None;
			Icon = string.Empty;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Effect"/> class with the specified effector, value, and duration.
		/// </summary>
		/// <param name="effector">The attribute, skill, or stat to be affected.</param>
		/// <param name="value">The amount to change the effector by.</param>
		/// <param name="duration">The duration of the effect. -1 for permanent effects.</param>
		public Effect(EffectTypes effector, sbyte value, short duration)
		{
			Effector = effector;
			Value = value;
			Icon = IconDeterminer.Determine(effector);
			Duration = duration;
			TimeLeft = Duration;
		}
		#endregion

		/// <summary>
		/// Returns a string representation of the effect, including its icon, effector, and time left.
		/// </summary>
		/// <returns>A string describing the effect.</returns>
		public override string ToString() => $"{Icon} {Effector}: {TimeLeft}";

		/// <summary>
		/// The possible attributes, skills, or player stats that can be changed by an <see cref="Effect"/>.
		/// </summary>
		public enum EffectTypes
		{
			/// <summary>
			/// No effect.
			/// </summary>
			None,

			#region SPECIAL
			/// <summary>Strength: Increases or decreases physical power, carry weight, and melee damage.</summary>
			Strength,
			/// <summary>Perception: Affects accuracy, detection, and lockpicking.</summary>
			Perception,
			/// <summary>Endurance: Modifies health, resistances, and stamina.</summary>
			Endurance,
			/// <summary>Charisma: Influences speech, barter, and companion effectiveness.</summary>
			Charisma,
			/// <summary>Intelligence: Affects skill points and hacking ability.</summary>
			Intelligence,
			/// <summary>Agility: Determines action points and sneaking ability.</summary>
			Agility,
			/// <summary>Luck: Affects critical hits and loot quality.</summary>
			Luck,
			#endregion

			#region Skills
			/// <summary>Barter: Skill in trading and negotiation.</summary>
			Barter,
			/// <summary>Energy Weapons: Proficiency with laser, plasma, and energy-based weapons.</summary>
			EnergyWeapons,
			/// <summary>Explosives: Skill with grenades, mines, and other explosives.</summary>
			Explosives,
			/// <summary>Gun: Proficiency with firearms (pistols, rifles, shotguns).</summary>
			Gun,
			/// <summary>Lockpick: Ability to open locked doors and containers.</summary>
			Lockpick,
			/// <summary>Medicine: Increases effectiveness of healing items.</summary>
			Medicine,
			/// <summary>Melee Weapons: Skill with hand-to-hand and bladed weapons.</summary>
			MeleeWeapons,
			/// <summary>Repair: Ability to fix and maintain equipment.</summary>
			Repair,
			/// <summary>Science: Skill in hacking and crafting advanced items.</summary>
			Science,
			/// <summary>Sneak: Ability to move undetected and perform sneak attacks.</summary>
			Sneak,
			/// <summary>Speech: Skill in persuasion and dialogue checks.</summary>
			Speech,
			/// <summary>Survival: Effectiveness with food, drink, and crafting survival items.</summary>
			Survival,
			/// <summary>Unarmed: Proficiency in hand-to-hand combat without weapons.</summary>
			Unarmed,
			#endregion

			#region Player
			/// <summary>Hit Points: The player's total health.</summary>
			HitPoints,
			/// <summary>Action Points: Used for actions in V.A.T.S. and special moves.</summary>
			ActionPoints,
			/// <summary>Damage Resistance: Reduces incoming damage from attacks.</summary>
			DamageResistance,
			#endregion
		}
	}
}
