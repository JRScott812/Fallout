using Pip_Boy_Classes.Objects;
using System;
using System.IO;
using System.Media;

namespace Pip_Boy
{
	/// <summary>
	/// Wrapper for Radio that handles audio playback in the app.
	/// </summary>
	public class RadioPlayer
	{
		private readonly SoundPlayer _soundPlayer = new();

		/// <summary>
		/// Initializes a new RadioPlayer instance.
		/// </summary>
		/// <param name="folderPath">Path to songs folder</param>
		public RadioPlayer(string folderPath)
		{
			Radio = new Radio(folderPath);
		}

		/// <summary>
		/// Gets the underlying radio object.
		/// </summary>
		public Radio Radio { get; }

		/// <summary>
		/// Plays the currently selected song.
		/// </summary>
		public void Play()
		{
			if (Radio.songs.Length > 0)
			{
				_soundPlayer.SoundLocation = Radio.songs[Radio.songIndex];
				_soundPlayer.Load();
				_soundPlayer.Play();
			}
		}

		/// <summary>
		/// Add a new song to the list using an inputted path.
		/// </summary>
		/// <param name="pipBoy">Used for error handling</param>
		public void AddSong(PipBoy pipBoy)
		{
			string? path = PipBoyInput.EnterValue<string>("Enter path to '.wav' file");

			if (path is not null && Path.GetExtension(path) == ".wav")
			{
				Array.Resize(ref Radio.songs, Radio.songs.Length + 1);
				Radio.songs[^1] = path;
			}
			else
			{
				pipBoy.Error("Please give a '.wav' file!");
			}
		}

		/// <summary>
		/// Changes the selected song.
		/// </summary>
		/// <param name="up">Whether to move up or down</param>
		public void ChangeSong(bool up)
		{
			Radio.ChangeSong(up);
		}

		/// <summary>
		/// Returns the radio's ToString representation.
		/// </summary>
		public override string ToString() => Radio.ToString();
	}
}
