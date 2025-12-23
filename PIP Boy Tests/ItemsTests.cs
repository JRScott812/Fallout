using Pip_Boy.Items;
using Pip_Boy.Objects.PIP_Boy;

namespace PIP_Boy_Tests
{
	[TestClass]
	public class ItemsTests
	{
		public static Item[] Items =
		[
			new Weapon("10mm Pistol", 5.5f, 55, [], Weapon.WeaponType.Gun, 3, 30, 10, 100),
			new TorsoPiece("Vault 13 Jumpsuit", 5, 25, [], 3, false),
			new HeadPiece("Nerd Goggles", 1, 25, [], 0, false),
			new Ammo("10mm Ammo", 1, [], Ammo.AmmoType.Bullet, Ammo.AmmoModification.Standard),
			new Aid("Stimpack", 1f, 30, [], Aid.AidType.Syringe),
			new Misc("Journal Entry", 1f, 15, Misc.MiscType.Other)
		];

		private static string serializedItemsFilesFolder = string.Empty;
		private static string[] serializedItemFilePaths = [];
		public static string[] serializedItemFiles => Directory.GetFiles(serializedItemsFilesFolder, "*.xml");

		[ClassInitialize]
		public static void ClassInitialize(TestContext context)
		{
			string projectDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
			serializedItemsFilesFolder = Path.Combine(projectDir, "Serialized Files", "Items") + Path.DirectorySeparatorChar;
			Directory.CreateDirectory(serializedItemsFilesFolder);

			serializedItemFilePaths = new string[Items.Length];

			for (int i = 0; i < Items.Length; i++)
			{
				serializedItemFilePaths[i] = PipBoySerializer.ToFile(serializedItemsFilesFolder, Items[i]);
			}
		}

		[TestMethod]
		public void ItemSerialization()
		{
			foreach (string filePath in serializedItemFilePaths)
			{
				Assert.IsTrue(serializedItemFiles.Contains(filePath));
			}
		}

		[TestMethod]
		public void ItemDeserialization()
		{
			// Check tag-to-type casting
			Type[] expectedTypes = [typeof(Weapon), typeof(TorsoPiece), typeof(HeadPiece), typeof(Ammo), typeof(Aid), typeof(Misc)];
			Type[] actualTypes = new Type[Items.Length];

			for (int i = 0; i < serializedItemFiles.Length; i++)
			{
				string filePath = serializedItemFiles[i];
				Type type = PipBoySerializer.GetTypeFromXML(filePath);
				actualTypes[i] = type;
			}

			for (int i = 0; i < expectedTypes.Length; i++)
			{
				Assert.IsTrue(actualTypes.Contains(expectedTypes[i]));
			}

			// Deserialize each file and compare to Items array by matching file name to item.Name
			foreach (var item in Items)
			{
				string expectedFileName = Path.Combine(serializedItemsFilesFolder, item.Name + ".xml");
				Assert.IsTrue(File.Exists(expectedFileName), $"Expected file not found: {expectedFileName}");

				Type type = PipBoySerializer.GetTypeFromXML(expectedFileName);
				Item? deserialized = type.Name switch
				{
					nameof(Weapon) => PipBoySerializer.FromFile<Weapon>(expectedFileName),
					nameof(TorsoPiece) => PipBoySerializer.FromFile<TorsoPiece>(expectedFileName),
					nameof(HeadPiece) => PipBoySerializer.FromFile<HeadPiece>(expectedFileName),
					nameof(Ammo) => PipBoySerializer.FromFile<Ammo>(expectedFileName),
					nameof(Aid) => PipBoySerializer.FromFile<Aid>(expectedFileName),
					nameof(Misc) => PipBoySerializer.FromFile<Misc>(expectedFileName),
					_ => throw new InvalidOperationException($"Unknown type: {type.Name}")
				};

				Assert.IsNotNull(deserialized, $"Deserialized item should not be null for {item.Name}");
				Assert.AreEqual(item.GetType(), deserialized.GetType(), $"Types should match for {item.Name}");
				Assert.AreEqual(item.Name, deserialized.Name, $"Names should match for {item.Name}");
				Assert.AreEqual(item.Weight, deserialized.Weight, $"Weights should match for {item.Name}");
				Assert.AreEqual(item.Value, deserialized.Value, $"Values should match for {item.Name}");
			}
		}
	}
}