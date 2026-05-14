using System.Runtime.Serialization;

namespace Pip_Boy_Classes.Entities.Mutants
{
	[DataContract]
	public class Ghoul : Mutant
	{
		/// <inheritdoc/>
		public Ghoul() : base() { }

		/// <inheritdoc/>
		public Ghoul(string name, byte level) : base(name, level)
		{
			Icon = "💀";
		}
	}
}
