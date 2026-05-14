using System.Runtime.Serialization;

namespace Pip_Boy_Classes.Entities.Robots
{
	[DataContract]
	public class Robot : Entity
	{
		/// <inheritdoc/>
		public Robot() : base() { }

		/// <inheritdoc/>
		public Robot(string name, byte level) : base(name, level)
		{
			headPiece = null;
			torsoPiece = null;
			weapon = null;
			Icon = "🤖";
		}
	}
}
