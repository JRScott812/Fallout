using System;
using System.Runtime.Serialization;

namespace Pip_Boy_Classes.Data_Types
{
	/// <summary>
	/// A permanent modifier to the <see cref="Pip_Boy_Classes.Entities.Player"/>'s attributes.
	/// </summary>
	[DataContract]
	public class Perk
	{
		/// <summary>
		/// What the <see cref="Perk"/> is called.
		/// </summary>
		[DataMember]
		public readonly string Name;

		/// <summary>
		/// A description of what the <see cref="Perk"/> does.
		/// </summary>
		[DataMember]
		public readonly string Description;

		/// <summary>
		/// What level the <see cref="Perk"/> is.
		/// </summary>
		[DataMember]
		public byte Level;

		/// <summary>
		/// If the <see cref="Perk"/> can be leveled up
		/// </summary>
		[DataMember]
		public readonly bool IsMultiLevel;

		#region Constructors
		/// <summary>
		/// Builds a perk with these variables.
		/// </summary>
		/// <param name="name">The <see cref="Name"/> to give the <see cref="Perk"/></param>
		/// <param name="description">The <see cref="Description"/> to give the <see cref="Perk"/></param>
		/// <param name="level">The <see cref="Level"/> to give the <see cref="Perk"/></param>
		/// <param name="multiLevel">If the <see cref="Perk"/> <see cref="IsMultiLevel"/>.</param>
		public Perk(string name, string description, byte level, bool multiLevel)
		{
			Name = name;
			Description = description;
			Level = level;
			IsMultiLevel = multiLevel;
		}

		/// <summary>
		/// Empty constructor for serialization.
		/// </summary>
		public Perk()
		{
			Name = string.Empty;
			Description = string.Empty;
		}
		#endregion

		/// <returns>The <see cref="Name"/>, <see cref="Level"/> (if !<see cref="IsMultiLevel"/>), and <see cref="Description"/>.</returns>
		public override string ToString()
		{
			string tempString = Name;
			if (IsMultiLevel)
			{
				tempString += "--Level:" + Level;
			}
			tempString += Environment.NewLine + '\t' + Description;
			return tempString;
		}
	}
}
