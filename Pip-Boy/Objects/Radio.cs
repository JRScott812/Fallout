using Pip_Boy.Objects.PIP_Boy;
using System;
using System.IO;
using System.Media;
using System.Text;

namespace Pip_Boy.Objects
{
	/// <summary>
	/// Contains all songs and logic for playing them.
	/// </summary>
	/// <remarks>
	/// Initialize a radio object with '.wav' files from a folder
	/// </remarks>
	/// <param name="folderPath">The path to the folder containing the songs</param>
	public class Radio(string folderPath)
	{
		/// <summary>
		/// What the songs will be played from.
		/// </summary>
		public SoundPlayer soundPlayer = new();

		/// <summary>
		/// The list of all <c>*.wav</c> audio files.
		/// </summary>
		public string[] songs = Directory.GetFiles(folderPath, "*.wav");

		/// <summary>
		/// Selected position in the <c>songs</c> list.
		/// </summary>
		public int songIndex = 0;

		/// <summary>
		/// Plays a random song from the list
		/// </summary>
		public void Play()
		{
			soundPlayer.SoundLocation = songs[songIndex];
			soundPlayer.Load();
			soundPlayer.Play();
		}

		#region Song Management
		/// <summary>
		/// Add a new song to the list using an inputted path
		/// </summary>
		/// <param name="pipBoy">Used for error handling</param>
		public void AddSong(PipBoy pipBoy)
		{
			string? path = PipBoyInput.EnterValue<string>("Enter path to '.wav' file");

			if (path is not null && Path.GetExtension(path) == ".wav")
			{
				Array.Resize(ref songs, songs.Length + 1);
				songs[^1] = path;
			}
			else
			{
				pipBoy.Error("Please give a '.wav' file!");
			}
		}

		/// <summary>
		/// Changes the selected song, and handles array bounds.
		/// </summary>
		/// <param name="up">Whether to move the index up or down</param>
		public void ChangeSong(bool up)
		{
			songIndex = up
				? (songIndex + 1) % songs.Length
				: (songIndex - 1 + songs.Length) % songs.Length;
		}

		#endregion

		/// <returns>A table with all the song names</returns>
		public override string ToString()
		{
			StringBuilder stringBuilder = new("Songs:\t" + songs.Length);
			stringBuilder.AppendLine();
			foreach (string song in songs)
			{
				stringBuilder.AppendLine('\t' + Path.GetFileNameWithoutExtension(song));
			}
			return stringBuilder.ToString();
		}
	}
}
