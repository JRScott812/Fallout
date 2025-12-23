using Pip_Boy.Data_Types;
using System.Runtime.Serialization;

namespace Pip_Boy.Items
{
	/// <summary>
	/// A Head armor
	/// </summary>
	[DataContract]
	public record HeadPiece : Apparel
	{
		#region Variable(s)
		#endregion

		#region Constructors
		/// <inheritdoc/>
		public HeadPiece(string name, float weight, ushort value, Effect[] effects, byte DT, bool powerArmor) : base(name, weight, value, effects, DT, powerArmor) { }

		/// <inheritdoc/>
		public HeadPiece() : base() { }
		#endregion

		#region Method(s)
		#endregion

		#region Enum(s)
		#endregion

		#region Override Functions
		#endregion
	}
}
