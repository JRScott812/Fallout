using System;
using System.IO;
using System.Linq;

using Pip_Boy_Classes.Entities;
using Pip_Boy_Classes.Objects;

namespace PIP_Boy_Tests
{
	[TestClass]
	public class PlayerTests
	{
		public static Player player = new();

		private static string serializedPlayerFileFolder = string.Empty;
		private static string serializedFilePath = string.Empty;

		[ClassInitialize]
		public static void ClassInitialize(TestContext context)
		{
			string projectDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
			serializedPlayerFileFolder = Path.Combine(projectDir, "Serialized Files", "Player") + Path.DirectorySeparatorChar;
			Directory.CreateDirectory(serializedPlayerFileFolder);
			player.Level = 7;
			for (int i = 0; i < player.SPECIAL.Length; i++)
			{
				player.SPECIAL[i].Value = (byte)(i + 1);
			}
			serializedFilePath = PipBoySerializer.ToFile(serializedPlayerFileFolder, player);
		}

		[TestMethod]
		public void PlayerSerialization()
		{
			Assert.IsTrue(File.Exists(serializedFilePath), $"Expected serialized file not found: {serializedFilePath}");
			Assert.AreEqual(player.GetType(), PipBoySerializer.GetTypeFromXML(serializedFilePath), "Deserialized type does not match expected Player type.");
		}

		[TestMethod]
		public void PlayerDeserialization()
		{
			Player deserializedPlayer = PipBoySerializer.FromFile<Player>(serializedFilePath);
			Assert.IsNotNull(deserializedPlayer, "Deserialized player should not be null.");
			Assert.AreEqual(player.GetType(), deserializedPlayer.GetType(), "Deserialized type does not match expected Player type.");
			Assert.AreEqual(player.Level, deserializedPlayer.Level, "Player level should be preserved.");
			Assert.AreSequenceEqual(
				[.. player.SPECIAL.Select(attribute => attribute.Value)], [.. deserializedPlayer.SPECIAL.Select(attribute => attribute.Value)], "Player SPECIAL attributes should be preserved.");
		}

		[TestMethod]
		public void LevelUp_IncreasesLevelAndGrantsPerkOnEvenLevels()
		{
			Player testPlayer = new();

			testPlayer.LevelUp();
			Assert.AreEqual((byte)1, testPlayer.Level);
			Assert.IsEmpty(testPlayer.Perks);

			testPlayer.LevelUp();
			Assert.AreEqual((byte)2, testPlayer.Level);
			Assert.HasCount(1, testPlayer.Perks);

			testPlayer.LevelUp();
			Assert.AreEqual((byte)3, testPlayer.Level);
			Assert.HasCount(1, testPlayer.Perks);
		}
	}
}
