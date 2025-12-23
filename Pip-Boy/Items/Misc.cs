using Pip_Boy.Data_Types;
using System;
using System.Runtime.Serialization;

namespace Pip_Boy.Items
{
	/// <summary>
	/// Can be junk, sellable items, or crafting components.
	/// </summary>
	[DataContract]
	public record Misc : Item
	{
		#region Variable(s)
		/// <summary>
		/// Represents the type of miscellaneous data associated with this instance.
		/// </summary>
		[DataMember]
		public readonly MiscType miscType;
		#endregion

		#region Constructors
		/// <inheritdoc/>
		public Misc(string name, float weight, ushort value, MiscType type) : base(name, weight, value)
		{
			miscType = type;
			Icon = IconDeterminer.Determine(type);
		}

		/// <inheritdoc/>
		public Misc() : base()
		{
			miscType = 0;
			Icon = string.Empty;
		}
		#endregion

		#region Method(s)
		#endregion

		#region Enum(s)
		/// <summary>
		/// The possible types for the <see cref="Misc"/> object.
		/// </summary>
		public enum MiscType
		{
			/// <summary>
			/// Any other type
			/// </summary>
			Other,

			/// <summary>
			/// Any useless item.
			/// </summary>
			Junk,

			/// <summary>
			/// Any valuable item.
			/// </summary>
			Sellable,

			/// <summary>
			/// Item which can be used to craft another item.
			/// </summary>
			Crafting,

			/// <summary>
			/// A key which can unlock doors
			/// </summary>
			Key,

			/// <summary>
			/// A package which can be delivered
			/// </summary>
			Package
		}
		#endregion
	}
}
