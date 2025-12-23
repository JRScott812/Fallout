using Pip_Boy.Data_Types;
using System.Runtime.Serialization;

namespace Pip_Boy.Items
{
	/// <summary>
	/// A torso armor
	/// </summary>
	[DataContract]
	public record TorsoPiece : Apparel
	{
		#region Variable(s)
		#endregion

		#region Constructors
		/// <inheritdoc/>
		public TorsoPiece(string name, float weight, ushort value, Effect[] effects, byte DT, bool powerArmor) : base(name, weight, value, effects, DT, powerArmor) { }

		/// <inheritdoc/>
		public TorsoPiece() : base() { }
		#endregion

		#region Method(s)
		#endregion

		#region Enum(s)
		#endregion

		#region Override Functions
		#endregion
	}
}
