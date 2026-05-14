using System.Runtime.Serialization;

namespace Pip_Boy_Classes.Entities.Creatures
{
	[DataContract]
	public class DeathClaw : Creature
	{
		/// <inheritdoc/>
		public DeathClaw() : base() { }

		/// <inheritdoc/>
		public DeathClaw(string name, byte level) : base(name, level)
		{
			Icon = "😈";
		}
	}
}
