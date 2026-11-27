using System;
using System.IO;
using System.Linq;
using System.Reflection;

using Pip_Boy_Classes.Entities;
using Pip_Boy_Classes.Entities.Creatures;
using Pip_Boy_Classes.Entities.Mutants;
using Pip_Boy_Classes.Entities.Robots;
using Pip_Boy_Classes.Objects;

namespace PIP_Boy_Tests
{
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

		[ClassInitialize]
		public static void ClassInitialize(TestContext context)
		{
			string projectDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
			serializedEntitiesFilesFolder = Path.Combine(projectDir, "Serialized Files", "Entities") + Path.DirectorySeparatorChar;
			Directory.CreateDirectory(serializedEntitiesFilesFolder);

			serializedEntityFilePaths = new string[Entities.Length];

			for (int i = 0; i < Entities.Length; i++)
			{
				serializedEntityFilePaths[i] = PipBoySerializer.ToFile(serializedEntitiesFilesFolder, Entities[i]);
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
				Assert.AreEqual(entity.GetType(), PipBoySerializer.GetTypeFromXML(filePath));
			}
		}

		[TestMethod]
		public void EntityDeserialization()
		{
			MethodInfo fromFile = typeof(PipBoySerializer).GetMethod(nameof(PipBoySerializer.FromFile))
				?? throw new InvalidOperationException("PipBoySerializer.FromFile<T> was not found.");

			foreach (string filePath in serializedEntityFilePaths)
			{
				Type type = PipBoySerializer.GetTypeFromXML(filePath);
				Entity expected = Entities.Single(entity => entity.GetType() == type);
				object? result = fromFile.MakeGenericMethod(type).Invoke(null, [filePath]);

				Assert.IsNotNull(result, $"Deserialized entity should not be null for {type.Name}");
				Assert.IsInstanceOfType(result, type, $"Deserialized type should match for {type.Name}");
				Assert.AreEqual(type, result.GetType(), $"Deserialized type should exactly match for {type.Name}");
				Entity deserialized = (Entity)result;
				Assert.AreEqual(expected.Name, deserialized.Name, $"Names should match for {type.Name}");
				Assert.AreEqual(expected.Level, deserialized.Level, $"Levels should match for {type.Name}");
				Assert.AreEqual(expected.CurrentHealth, deserialized.CurrentHealth, $"Health should match for {type.Name}");
				Assert.AreSequenceEqual(
					[.. expected.SPECIAL.Select(attribute => attribute.Value)], [.. deserialized.SPECIAL.Select(attribute => attribute.Value)], $"SPECIAL attributes should match for {type.Name}");
			}
		}
	}
}