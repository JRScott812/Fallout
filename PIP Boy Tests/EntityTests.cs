using Pip_Boy.Entities;
using Pip_Boy.Entities.Creatures;
using Pip_Boy.Entities.Mutants;
using Pip_Boy.Entities.Robots;
using Pip_Boy.Objects;

namespace PIP_Boy_Tests;

[TestClass]
public class EntityTests
{
	public static Entity[] Entities =
	[
		new Human(),
		new Player(),

		new Robot(),

		new Ghoul(),
		new Feral(),

		new SuperMutant(),
		new Nightkin(),

		new DeathClaw(),

		new Dog(),
		new NightStalker(),

		new BloatFly()
	];

	private static string serializedEntitiesFilesFolder = string.Empty;
	private static string[] serializedEntityFilePaths = [];
	public static string[] serializedEntityFiles => Directory.GetFiles(serializedEntitiesFilesFolder, "*.xml");

	[ClassInitialize]
	public static void ClassInitialize(TestContext context)
	{
		string testRunDir = context.TestRunDirectory ?? Directory.GetCurrentDirectory();
		serializedEntitiesFilesFolder = Path.Combine(testRunDir, "Serialized Files", "Entities") + Path.DirectorySeparatorChar;
		Directory.CreateDirectory(serializedEntitiesFilesFolder);

		serializedEntityFilePaths = new string[Entities.Length];

		for (int i = 0; i < Entities.Length; i++)
		{
			serializedEntityFilePaths[i] = PipBoy.ToFile(serializedEntitiesFilesFolder, Entities[i]);
		}
	}

	[TestMethod]
	public void EntitySerialization()
	{
		foreach (string filePath in serializedEntityFilePaths)
		{
			// Find the corresponding entity by matching the filename
			string fileName = Path.GetFileNameWithoutExtension(filePath);
			Entity? entity = Entities.FirstOrDefault(e => e.GetType().Name == fileName);
			
			Assert.IsNotNull(entity, $"Could not find entity for file: {fileName}");
			Assert.AreEqual(entity.GetType(), PipBoy.GetTypeFromXML(filePath));
		}
	}

	[TestMethod]
	public void EntityDeserialization()
	{
		foreach (string filePath in serializedEntityFiles)
		{
			Type type = PipBoy.GetTypeFromXML(filePath);
			Entity? deserialized = (Entity?)Activator.CreateInstance(type);
			Assert.IsNotNull(deserialized);
			Assert.AreEqual(type, deserialized.GetType());
		}
	}
}
