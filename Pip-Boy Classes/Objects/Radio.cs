using System.IO;
using System.Text;

namespace Pip_Boy_Classes.Objects
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
		/// The list of all <c>*.wav</c> audio files.
		/// </summary>
		public string[] songs = Directory.GetFiles(folderPath, "*.wav");

		/// <summary>
		/// Selected position in the <c>songs</c> list.
		/// </summary>
		public int songIndex = 0;

		#region Song Management
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
