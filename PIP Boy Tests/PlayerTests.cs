using Pip_Boy.Entities;
using Pip_Boy.Objects;

namespace PIP_Boy_Tests;

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
		serializedFilePath = PipBoy.ToFile(serializedPlayerFileFolder, player);
	}

	[TestMethod]
	public void PlayerSerialization()
	{
		Assert.IsTrue(Directory.EnumerateFiles(serializedPlayerFileFolder).Contains(serializedFilePath));
		Assert.AreEqual(player.GetType(), PipBoy.GetTypeFromXML(serializedFilePath), "Deserialized type does not match expected Player type.");
	}

	[TestMethod]
	public void PlayerDeserialization()
	{
		string filePath = Directory.GetFiles(serializedPlayerFileFolder, "*.xml")[0];
		Player deserializedPlayer = PipBoy.FromFile<Player>(filePath);
		Assert.IsNotNull(deserializedPlayer, "Deserialized player should not be null.");
		Assert.AreEqual(player.GetType(), deserializedPlayer.GetType(), "Deserialized type does not match expected Player type.");
	}
}
