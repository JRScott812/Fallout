using Pip_Boy.Data_Types;
using System.Runtime.Serialization;

namespace Pip_Boy.Items
{
	/// <summary>
	/// Represents a wearable item, such as armor, that can reduce damage taken by the player or entity.
	/// </summary>
	[DataContract]
	public abstract record Apparel : Equipable
	{
		#region Variable(s)
		/// <summary>
		/// The original damage threshold value of the apparel before condition is applied.
		/// </summary>
		[DataMember]
		private readonly byte originalDamageThreshold;

		/// <summary>
		/// The type of armor piece (e.g., head, torso) this apparel represents.
		/// </summary>
		[DataMember]
		public readonly PieceType pieceType;

		/// <summary>
		/// Gets the current damage threshold, which is the amount of damage this apparel will negate, factoring in its condition.
		/// </summary>
		public byte DamageThreshold => (byte)(originalDamageThreshold * Condition);

		/// <summary>
		/// Indicates whether this apparel requires the "Power Armor Training" <see cref="Perk"/> to be worn.
		/// </summary>
		[DataMember]
		public readonly bool RequiresPowerArmorTraining;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Apparel"/> class with the specified parameters.
		/// </summary>
		/// <param name="name">The name of the apparel item.</param>
		/// <param name="weight">The weight of the apparel item.</param>
		/// <param name="value">The value (in caps) of the apparel item.</param>
		/// <param name="effects">The effects this apparel applies when equipped.</param>
		/// <param name="DT">The original damage threshold value.</param>
		/// <param name="powerArmor">Whether this apparel requires Power Armor Training.</param>
		public Apparel(string name, float weight, ushort value, Effect[] effects, byte DT, bool powerArmor) : base(name, weight, value, effects)
		{
			originalDamageThreshold = DT;
			RequiresPowerArmorTraining = powerArmor;
			pieceType = GetPieceType(this);
			Icon = IconDeterminer.Determine(this);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Apparel"/> class for serialization.
		/// </summary>
		public Apparel() : base()
		{
			originalDamageThreshold = 0;
			RequiresPowerArmorTraining = false;
			pieceType = 0;
			Icon = string.Empty;
		}
		#endregion

		#region Method(s)
		/// <summary>
		/// Determines the <see cref="PieceType"/> of the given <see cref="Apparel"/> based on its weight and type.
		/// </summary>
		/// <param name="apparel">The apparel item to evaluate.</param>
		/// <returns>The determined <see cref="PieceType"/> (Nothing, Light, Medium, or Heavy).</returns>
		public static PieceType GetPieceType(Apparel apparel)
		{
			int nothingWeight = apparel is HeadPiece ? 1 : 10;
			int lightWeight = apparel is HeadPiece ? 2 : 25;
			int mediumWeight = apparel is HeadPiece ? 7 : 30;

			return apparel.Weight switch
			{
				_ when apparel.Weight <= nothingWeight => PieceType.Nothing,
				_ when apparel.Weight <= lightWeight => PieceType.Light,
				_ when apparel.Weight <= mediumWeight => PieceType.Medium,
				_ => PieceType.Heavy,
			};
		}
		#endregion

		#region Enum(s)
		/// <summary>
		/// Represents the approximate damage threshold category that the <see cref="Apparel"/> provides.
		/// </summary>
		public enum PieceType : byte
		{
			/// <summary>
			/// No significant protection (e.g., clothing or very light gear).
			/// </summary>
			Nothing,
			/// <summary>
			/// Light armor, offering minimal protection but high mobility.
			/// </summary>
			Light,
			/// <summary>
			/// Medium armor, balancing protection and mobility.
			/// </summary>
			Medium,
			/// <summary>
			/// Heavy armor, offering maximum protection but reduced mobility.
			/// </summary>
			Heavy
		}
		#endregion
	}
}
