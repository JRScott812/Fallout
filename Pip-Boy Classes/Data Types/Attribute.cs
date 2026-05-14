using System.Runtime.Serialization;

namespace Pip_Boy_Classes.Data_Types
{
	/// <summary>
	/// Represents a player or entity attribute, such as Strength, Perception, or a skill.
	/// Contains the attribute's name, value, and an associated icon.
	/// </summary>
	[DataContract]
	public class Attribute(Attribute.AttributeName name, byte value)
	{
		/// <summary>
		/// The name of the <see cref="Attribute"/>
		/// </summary>
		[DataMember]
		public readonly AttributeName Name = name;

		/// <summary>
		/// The numeric value of the <see cref="Attribute"/>.
		/// </summary>
		[DataMember]
		public byte Value = value;

		/// <summary>
		/// The emoji icon representing the <see cref="Attribute"/>.
		/// </summary>
		[DataMember]
		public readonly string Icon = IconDeterminer.Determine(name);

		/// <summary>
		/// Returns a string representation of the attribute, including its name, icon, and value.
		/// </summary>
		/// <returns>A string in the format "NameIcon:	Value".</returns>
		public override string ToString() => $"{Name}{Icon}:\t{Value}";

		/// <summary>
		/// Enumerates all possible attribute types, including S.P.E.C.I.A.L. stats and skills.
		/// </summary>
		public enum AttributeName
		{
			/// <summary>
			/// Strength: A measure of your raw physical power. Affects how much you can carry, melee damage, and the ability to use heavy weapons.
			/// </summary>
			Strength,

			/// <summary>
			/// Perception: Your environmental awareness and "sixth sense." Affects weapon accuracy in V.A.T.S., lockpicking, and detection of enemies.
			/// </summary>
			Perception,

			/// <summary>
			/// Endurance: A measure of your overall physical fitness. Determines your total health, resistance to radiation and poison, and stamina.
			/// </summary>
			Endurance,

			/// <summary>
			/// Charisma: Your ability to charm and convince others. Influences speech checks, barter prices, and companion effectiveness.
			/// </summary>
			Charisma,

			/// <summary>
			/// Intelligence: A measure of your overall mental acuity. Affects skill points gained per level and success in hacking terminals.
			/// </summary>
			Intelligence,

			/// <summary>
			/// Agility: A measure of your overall finesse and reflexes. Determines the number of Action Points in V.A.T.S. and affects sneaking.
			/// </summary>
			Agility,

			/// <summary>
			/// Luck: A measure of your general good fortune. Affects critical hit chance and the quality of loot you find.
			/// </summary>
			Luck,

			/// <summary>
			/// Barter: Skill in trading and negotiation. Reduces item prices when buying and increases prices when selling.
			/// </summary>
			Barter,

			/// <summary>
			/// Energy Weapons: Proficiency with laser, plasma, and other energy-based weapons. Increases damage and accuracy with these weapons.
			/// </summary>
			EnergyWeapons,

			/// <summary>
			/// Explosives: Skill in using grenades, mines, and other explosive devices. Increases damage and effectiveness of explosives.
			/// </summary>
			Explosives,

			/// <summary>
			/// Gun: Proficiency with firearms such as pistols, rifles, and shotguns. Increases damage and accuracy with guns.
			/// </summary>
			Gun,

			/// <summary>
			/// Lockpick: Skill in opening locked doors and containers. Higher skill allows picking more difficult locks.
			/// </summary>
			Lockpick,

			/// <summary>
			/// Medicine: Knowledge of medical practices. Increases the effectiveness of stimpaks and other healing items.
			/// </summary>
			Medicine,

			/// <summary>
			/// Melee Weapons: Proficiency with hand-to-hand and bladed weapons. Increases damage and effectiveness with melee weapons.
			/// </summary>
			MeleeWeapons,

			/// <summary>
			/// Repair: Skill in maintaining and fixing equipment. Allows you to repair weapons and armor more effectively.
			/// </summary>
			Repair,

			/// <summary>
			/// Science: Knowledge of computers and technology. Used for hacking terminals and crafting advanced items.
			/// </summary>
			Science,

			/// <summary>
			/// Sneak: Ability to move quietly and remain undetected. Affects your ability to avoid detection and perform sneak attacks.
			/// </summary>
			Sneak,

			/// <summary>
			/// Speech: Skill in persuasion and conversation. Increases success in dialogue checks and negotiations.
			/// </summary>
			Speech,

			/// <summary>
			/// Survival: Knowledge of wilderness survival techniques. Improves effectiveness of food, drink, and crafting survival items.
			/// </summary>
			Survival,

			/// <summary>
			/// Unarmed: Proficiency in hand-to-hand combat without weapons. Increases damage and effectiveness of unarmed attacks.
			/// </summary>
			Unarmed,
		}
	}
}
