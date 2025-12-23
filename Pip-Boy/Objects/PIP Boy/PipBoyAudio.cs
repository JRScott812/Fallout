using System;
using System.IO;
using System.Media;
using System.Threading;

namespace Pip_Boy.Objects.PIP_Boy
{
	/// <summary>
	/// Manages audio playback for the PIP-Boy, including sound effects and boot/shutdown sequences.
	/// </summary>
	/// <remarks>
	/// Initializes a new instance of the <see cref="PipBoyAudio"/> class.
	/// </remarks>
	/// <param name="workingDirectory">The root directory containing sound files.</param>
	public class PipBoyAudio(string workingDirectory)
	{
		#region Constants
		/// <summary>
		/// Index of the menu navigation sound in the sounds array.
		/// </summary>
		private const int MENU_SOUND_INDEX = 1;

		/// <summary>
		/// Index of the faction navigation up sound in the sounds array.
		/// </summary>
		private const int FACTION_UP_SOUND_INDEX = 2;

		/// <summary>
		/// Index of the faction navigation down sound in the sounds array.
		/// </summary>
		private const int FACTION_DOWN_SOUND_INDEX = 3;

		/// <summary>
		/// Index of the boot sound in the sounds array.
		/// </summary>
		private const int BOOT_SOUND_INDEX = 2;

		/// <summary>
		/// Index of the loading sound in the sounds array.
		/// </summary>
		private const int LOADING_SOUND_INDEX = 6;

		/// <summary>
		/// Index of the startup complete sound in the sounds array.
		/// </summary>
		private const int STARTUP_COMPLETE_SOUND_INDEX = 8;
		#endregion

		#region Properties
		/// <summary>
		/// The sound player used for all audio playback.
		/// </summary>
		private readonly SoundPlayer _soundPlayer = new();

		/// <summary>
		/// Sound effects array for UI interactions.
		/// </summary>
		public string[] Sounds { get; } = Directory.GetFiles(Path.Combine(workingDirectory, "Sounds"), "*.wav");

		/// <summary>
		/// Sounds for static between songs and menu navigation
		/// </summary>
		public string[] StaticSounds { get; } = Directory.GetFiles(Path.Combine(workingDirectory, "Sounds", "static"), "*.wav");

		/// <summary>
		/// Geiger click sounds, for when in the RAD menu
		/// </summary>
		public string[] RadiationSounds { get; } = Directory.GetFiles(Path.Combine(workingDirectory, "Sounds", "radiation"), "*.wav");

		#endregion
		#region Constructor
		#endregion

		#region Sound Playback
		/// <summary>
		/// Play a sound effect from a file path.
		/// </summary>
		/// <param name="path">The path to the <c>*.wav</c> file.</param>
		public void PlaySound(string path)
		{
			_soundPlayer.SoundLocation = path;
			_soundPlayer.Load();
			_soundPlayer.Play();
		}

		/// <summary>
		/// Plays the menu navigation sound.
		/// </summary>
		public void PlayMenuSound()
		{
			PlaySound(Sounds[^MENU_SOUND_INDEX]);
		}

		/// <summary>
		/// Plays the faction navigation up sound.
		/// </summary>
		public void PlayFactionUpSound()
		{
			PlaySound(Sounds[^FACTION_UP_SOUND_INDEX]);
		}

		/// <summary>
		/// Plays the faction navigation down sound.
		/// </summary>
		public void PlayFactionDownSound()
		{
			PlaySound(Sounds[^FACTION_DOWN_SOUND_INDEX]);
		}
		#endregion

		#region Boot/Shutdown Sequences
		/// <summary>
		/// Displays and plays the PIP-Boy boot sequence.
		/// </summary>
		public void PlayBootSequence()
		{
			PlaySound(Sounds[BOOT_SOUND_INDEX]);

			SlowType("PIP-Boy 3000 MKIV");
			SlowType("Copyright 2075 RobCo Industries");
			SlowType("64kb Memory");
			SlowType(new string('-', Console.WindowWidth));

			PlaySound(Sounds[LOADING_SOUND_INDEX]);

			Console.Clear();
			Console.WriteLine();
			SlowType("VAULT-TEC");
			Thread.Sleep(1250);

			Console.Clear();
			SlowType("LOADING...");
			Thread.Sleep(2500);
			Console.Clear();

			PlaySound(Sounds[STARTUP_COMPLETE_SOUND_INDEX]);
		}

		/// <summary>
		/// Displays and plays the PIP-Boy shutdown sequence.
		/// </summary>
		public void PlayShutdownSequence()
		{
			PlaySound(Sounds[BOOT_SOUND_INDEX]);

			SlowType("Shutting Down...");
			SlowType(new string('-', Console.WindowWidth));
			Thread.Sleep(1000);
		}
		#endregion

		#region Helper Methods
		/// <summary>
		/// Slowly types out a message to the console with a typewriter effect.
		/// </summary>
		/// <param name="message">The message to display.</param>
		private static void SlowType(string message)
		{
			foreach (char c in message)
			{
				Console.Write(c);
				Thread.Sleep(5);
			}
			Console.WriteLine();
		}
		#endregion
	}
}
