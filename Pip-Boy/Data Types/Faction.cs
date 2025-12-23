using System.Numerics;
using System.Runtime.Serialization;

namespace Pip_Boy.Data_Types
{
	/// <summary>
	/// The faction object which can have reputation.
	/// </summary>
	[DataContract]
	public class Faction(string name, string description)
	{
		/// <summary>
		/// The faction's name.
		/// </summary>
		[DataMember]
		public readonly string Name = name;

		/// <summary>
		/// The faction's description.
		/// </summary>
		[DataMember]
		public readonly string Description = description;

		/// <summary>
		/// The reputation value.
		/// <see cref="Vector2.X"/> represents the positive reputation.
		/// <see cref="Vector2.Y"/> represents the negative reputation.
		/// </summary>
		[DataMember]
		public Vector2 Reputation = new();

		/// <summary>
		/// The type of reputation, based on the <see cref="Reputation"/>
		/// </summary>
		[DataMember]
		public Reputations reputation = Reputations.Neutral;

		/// <summary>
		/// The levels of reputation.
		/// </summary>
		public enum Reputations : sbyte
		{
			/// <summary>
			/// For your overwhelmingly monstrous behavior, you have become vilified by the community
			/// </summary>
			Vilified = -128,

			/// <summary>
			/// Now that folks know you're bad, most people outright hate you.
			/// </summary>
			Hated = -84,

			/// <summary>
			/// You've left a poor impression on the community and may be shunned as a result.
			/// </summary>
			Shunned = -42,

			/// <summary>
			/// People don't know enough about you to form an opinion.
			/// </summary>
			Neutral = 0,

			/// <summary>
			/// Folks have come to accept you for your helpful nature.
			/// </summary>
			Accepted = 41,

			/// <summary>
			/// Enough news of your good works has been passed around that people like you.
			/// </summary>
			Liked = 80,

			/// <summary>
			/// Renowned for your extensive support and goodwill, you are idolized by the community.
			/// </summary>
			Idolized = 127
		}

		/// <returns>The <see cref="Faction"/>'s <see cref="Name"/> and <see cref="reputation"/></returns>
		public override string ToString() => $"{Name}:\t{reputation}";
	}
}
