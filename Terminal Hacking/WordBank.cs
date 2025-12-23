namespace Terminal_Minigame
{
	/// <summary>
	/// Stores word banks for different difficulty levels in the terminal hacking minigame.
	/// </summary>
	internal readonly struct WordBank()
	{
		/// <summary>
		/// Words with length of 4-5
		/// </summary>
		readonly string[] veryEasy = [];

		/// <summary>
		/// Words with length of 6-8
		/// </summary>
		readonly string[] easy = [];

		/// <summary>
		/// Words with length of 9-10
		/// </summary>
		readonly string[] average = [];

		/// <summary>
		/// Words with length of 11-12
		/// </summary>
		readonly string[] hard = [];

		/// <summary>
		/// Words with length of 13-15
		/// </summary>
		readonly string[] veryHard = [];
	}
}
