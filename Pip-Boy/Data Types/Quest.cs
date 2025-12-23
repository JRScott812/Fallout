using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;

namespace Pip_Boy.Data_Types
{
	/// <summary>
	/// A journey that needs to be completed
	/// </summary>
	/// <param name="name"></param>
	/// <param name="steps"></param>
	[DataContract]
	public class Quest(string name, List<Quest.Step> steps)
	{
		/// <summary>
		/// The Name of the <see cref="Quest"/>
		/// </summary>
		[DataMember]
		public readonly string Name = name;

		/// <summary>
		/// List of all the <see cref="Step"/>s, which must be completed
		/// </summary>
		[DataMember]
		public List<Step> Steps = steps;

		/// <summary>
		/// Parts of a <see cref="Quest"/> that need to be completed
		/// </summary>
		/// <param name="instructions"></param>
		/// <param name="position"></param>
		/// <param name="optional"></param>
		[DataContract]
		public class Step(string instructions, Vector2 position, bool optional)
		{
			/// <summary>
			/// What must be done for the <see cref="Step"/> to be completed
			/// </summary>
			[DataMember]
			public readonly string Instructions = instructions;

			/// <summary>
			/// If the step is completed or not
			/// </summary>
			[DataMember]
			public bool Completed = false;

			/// <summary>
			/// If the step is optional, this can't be changed after construction
			/// </summary>
			[DataMember]
			public readonly bool Optional = optional;

			/// <summary>
			/// If the step is hidden, when the previous step is completed, <see cref="Completed"/> will be <c>true</c> and <see cref="Hidden"/> will be <c>false</c>.
			/// </summary>
			[DataMember]
			public bool Hidden = true;

			/// <summary>
			/// The location of the <see cref="Quest"/>'s <see cref="Step"/> on the <see cref="Objects.Map"/>
			/// </summary>
			[DataMember]
			public readonly Vector2 Position = position;
		}

		/// <returns>The <see cref="Quest"/> and all of the <see cref="Steps"/></returns>
		public override string ToString()
		{
			StringBuilder stringBuilder = new(Name);
			stringBuilder.AppendLine();
			foreach (Step step in Steps)
			{
				if (!step.Hidden)
				{
					stringBuilder.AppendLine('\t' + step.Instructions);
				}
			}
			return stringBuilder.ToString();
		}
	}
}
