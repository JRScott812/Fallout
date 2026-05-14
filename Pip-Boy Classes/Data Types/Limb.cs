using System.Runtime.Serialization;

namespace Pip_Boy_Classes.Data_Types
{
	/// <summary>
	/// Body part of an <see cref="Pip_Boy_Classes.Entities.Entity"/>.
	/// </summary>
	/// <param name="name">The Limb's name which will be displayed.</param>
	/// <param name="icon">The emoji icon to display.</param>
	/// <param name="crippledEffects">What happens when the limb is crippled.</param>
	[DataContract]
	public class Limb(string name, string icon, Effect[] crippledEffects)
	{
		/// <summary>
		/// The name of the <see cref="Limb"/>.
		/// </summary>
		[DataMember]
		public readonly string Name = name;
		/// <summary>
		/// An emoji icon representing the <see cref="Limb"/>.
		/// </summary>
		[DataMember]
		public readonly string Icon = icon;

		/// <summary>
		/// How close the <see cref="Limb"/> is to being crippled.
		/// </summary>
		[DataMember]
		public float Condition { get; private set; } = 1f;

		/// <summary>
		/// If the <see cref="Limb"/> is crippled.
		/// </summary>
		public bool IsCrippled { get => Condition <= 0.25f; }

		/// <summary>
		/// What <see cref="Effect"/>s to apply when the <see cref="Limb"/> is crippled.
		/// </summary>
		[DataMember]
		readonly Effect[] CrippledEffects = crippledEffects;
	}
}
