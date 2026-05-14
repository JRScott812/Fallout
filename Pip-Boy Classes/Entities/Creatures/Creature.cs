using System.Runtime.Serialization;

namespace Pip_Boy_Classes.Entities.Creatures
{
	[DataContract]
	public abstract class Creature : Entity
	{
		/// <inheritdoc/>
		public Creature() : base() { }

		/// <inheritdoc/>
		public Creature(string name, byte level) : base(name, level) { }
	}
}
