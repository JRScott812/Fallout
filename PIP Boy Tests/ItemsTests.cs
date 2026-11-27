using System;
using System.IO;
using System.Reflection;

using Pip_Boy_Classes.Items;
using Pip_Boy_Classes.Objects;

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
				Assert.IsTrue(File.Exists(filePath), $"Expected serialized file not found: {filePath}");
			}
		}

		[TestMethod]
		public void ItemDeserialization()
		{
			MethodInfo fromFile = typeof(PipBoySerializer).GetMethod(nameof(PipBoySerializer.FromFile))
				?? throw new InvalidOperationException("PipBoySerializer.FromFile<T> was not found.");

			for (int i = 0; i < Items.Length; i++)
			{
				Item expected = Items[i];
				string filePath = serializedItemFilePaths[i];
				Type type = PipBoySerializer.GetTypeFromXML(filePath);
				Assert.AreEqual(expected.GetType(), type, $"Serialized type should match for {expected.Name}");

				object? result = fromFile.MakeGenericMethod(type).Invoke(null, [filePath]);
				Assert.IsNotNull(result, $"Deserialized item should not be null for {expected.Name}");
				Assert.IsInstanceOfType(result, type, $"Deserialized type should match for {expected.Name}");
				Assert.AreEqual(type, result.GetType(), $"Deserialized type should exactly match for {expected.Name}");
				Item deserialized = (Item)result;
				Assert.AreEqual(expected.Name, deserialized.Name, $"Names should match for {expected.Name}");
				Assert.AreEqual(expected.Weight, deserialized.Weight, $"Weights should match for {expected.Name}");
				Assert.AreEqual(expected.Value, deserialized.Value, $"Values should match for {expected.Name}");
			}
		}
	}
}