using System.Runtime.Serialization;
using System.Text;

namespace Pip_Boy_Classes.Entities
{
	/// <summary>
	/// Entities with 2 arms and 2 legs
	/// </summary>
	[DataContract]
	public abstract class Humanoid : Entity
	{
		/// <summary>
		/// How much money the <see cref="Entity"/> has.
		/// </summary>
		[DataMember]
		public ushort Caps;

		/// <inheritdoc/>
		public Humanoid() : base() { }

		/// <inheritdoc/>
		public Humanoid(string name, byte level) : base(name, level)
		{
			Limbs = [
				new("Head", "😐", []),
				new("Left Arm", "💪", []),
				new("Torso", "👕", []),
				new("Right Arm", "💪", []),
				new("Left Leg", "🦵", []),
				new("Right Leg", "🦵", [])
			];
		}

		/// <inheritdoc/>
		public override string ShowLimbs()
		{
			StringBuilder stringBuilder = new(Limbs[0].Icon);
			stringBuilder.AppendLine(Limbs[1].Icon + Limbs[2].Icon + Limbs[3].Icon);
			stringBuilder.AppendLine(Limbs[4].Icon + Limbs[5].Icon);
			return stringBuilder.ToString();
		}
	}
}
