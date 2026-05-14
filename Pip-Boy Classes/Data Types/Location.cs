using System;
using System.Numerics;
using System.Runtime.Serialization;

namespace Pip_Boy_Classes.Data_Types
{
	/// <summary>
	/// A point of interest on the <see cref="Pip_Boy_Classes.Objects.Map"/>
	/// </summary>
	[DataContract]
	public class Location
	{
		/// <summary>
		/// The <see cref="Location"/>'s name.
		/// </summary>
		[DataMember]
		public readonly string Name;

		/// <summary>
		/// A description of the <see cref="Location"/>.
		/// </summary>
		[DataMember]
		public readonly string Description;

		/// <summary>
		/// The <see cref="Location"/>'s icon.
		/// </summary>
		[DataMember]
		public readonly string Icon;

		/// <summary>
		/// How many rads per second the location has.
		/// </summary>
		[DataMember]
		public readonly byte Rads;

		/// <summary>
		/// Where the <see cref="Location"/> is on the <see cref="Pip_Boy_Classes.Objects.Map"/>.
		/// </summary>
		[DataMember]
		public readonly Vector2 Position;

		#region Constructors
		/// <summary>
		/// Construct a <see cref="Location"/> manually.
		/// </summary>
		/// <param name="name">The <see cref="Name"/> of the <see cref="Location"/></param>
		/// <param name="description">The <see cref="Description"/> that describe the <see cref="Location"/></param>.
		/// <param name="icon">The <see cref="Icon"/> to represent the <see cref="Location"/> with.</param>
		/// <param name="rads">How radiated the <see cref="Location"/> is.</param>
		/// <param name="position">Where the <see cref="Location"/> is on the <see cref="Pip_Boy_Classes.Objects.Map"/>.</param>
		public Location(string name, string description, string icon, byte rads, Vector2 position)
		{
			Name = name;
			Description = description;
			Icon = icon;
			Rads = rads;
			Position = position;
		}

		/// <summary>
		/// Empty Constructer for serialization.
		/// </summary>
		public Location()
		{
			Name = string.Empty;
			Description = string.Empty;
			Icon = string.Empty;
		}
		#endregion

		/// <summary>
		/// A string representation of the <see cref="Location"/>.
		/// </summary>
		/// <returns>A string representation of the <see cref="Location"/>.</returns>
		public override string ToString() => $"{Icon} {Name}:{Environment.NewLine}\t{Description}";
	}
}
