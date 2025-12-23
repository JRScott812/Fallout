using Pip_Boy.Data_Types;
using Pip_Boy.Entities;
using Pip_Boy.Entities.Creatures;
using Pip_Boy.Entities.Mutants;
using Pip_Boy.Entities.Robots;
using Pip_Boy.Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;

namespace Pip_Boy.Objects.PIP_Boy
{
	/// <summary>
	/// Provides XML serialization and deserialization utilities for PIP-Boy game objects.
	/// </summary>
	public static class PipBoySerializer
	{
		/// <summary>
		/// Serializes the <see cref="object"/> to an <c>*.xml</c> file.
		/// </summary>
		/// <param name="folderPath">The folder to write the <c>*.xml</c> file to.</param>
		/// <param name="obj">The <see cref="object"/> to serialize.</param>
		/// <returns>The full path to the created XML file.</returns>
		/// <exception cref="DirectoryNotFoundException">If the folder path does not exist.</exception>
		/// <exception cref="ArgumentException">If the object is not a serializable type.</exception>
		public static string ToFile(string folderPath, object obj)
		{
			if (Directory.Exists(folderPath))
			{
				Type type = obj.GetType();
				string name = obj switch
				{
					Item item => item.Name,
					Entity entity => entity.Name,
					Perk perk => perk.Name,
					Location location => location.Name,
					_ => throw new ArgumentException("Object must be Item, Entity, Perk, or Location type.", nameof(obj))
				};

				if (string.IsNullOrEmpty(name))
				{
					name = type.Name;
				}

				string filePath = Path.Combine(folderPath, $"{name}.xml");
				DataContractSerializer serializer = new(type);

				XmlWriterSettings writerSettings = new()
				{
					Indent = true,
					IndentChars = "\t",
					NewLineChars = "\n",
					NewLineHandling = NewLineHandling.Replace,
					OmitXmlDeclaration = false,
					NewLineOnAttributes = false,
					CloseOutput = true,
				};

				using (XmlWriter writer = XmlWriter.Create(filePath, writerSettings))
				{
					writer.WriteProcessingInstruction("xml-stylesheet", "type=\"text/css\" href=\"..\\Inventory Styling.css\"");
					serializer.WriteObject(writer, obj);
				}

				return filePath;
			}
			throw new DirectoryNotFoundException($"Folder not found: {folderPath}");
		}

		/// <summary>
		/// Deserializes an object from an <c>*.xml</c> file.
		/// </summary>
		/// <typeparam name="T">The type to deserialize to</typeparam>
		/// <param name="filePath">The path to the <c>*.xml</c> file.</param>
		/// <returns>The deserialized object.</returns>
		/// <exception cref="FileNotFoundException">If the file does not exist.</exception>
		/// <exception cref="FileLoadException">If the file is not a valid XML file.</exception>
		/// <exception cref="NullReferenceException">If the <c>*.xml</c> file returns a null object.</exception>
		public static T FromFile<T>(string filePath) where T : notnull
		{
			if (File.Exists(filePath))
			{
				if (Path.GetExtension(filePath) == ".xml")
				{
					DataContractSerializer serializer = new(typeof(T));

					XmlReaderSettings readerSettings = new()
					{
						IgnoreWhitespace = true,
						IgnoreComments = true,
						IgnoreProcessingInstructions = true,
						CheckCharacters = true,
						DtdProcessing = DtdProcessing.Ignore,
						ValidationType = ValidationType.None,
						CloseInput = true,
					};

					using XmlReader reader = XmlReader.Create(filePath, readerSettings);
					object? result = serializer.ReadObject(reader);
					return (T)(result ?? throw new NullReferenceException("Deserialized object is null."));
				}
				throw new FileLoadException("File is not '*.xml'. ", filePath);
			}
			throw new FileNotFoundException("File not found. ", filePath);
		}

		/// <summary>
		/// Reads the root tag of an <c>*.xml</c> file to determine the object type.
		/// </summary>
		/// <param name="filePath">The path to the file</param>
		/// <returns>The <see cref="Type"/> from the tag name.</returns>
		/// <exception cref="FileNotFoundException">If the file does not exist.</exception>
		/// <exception cref="FormatException">If the file is not <c>*.xml</c>.</exception>
		/// <exception cref="NullReferenceException">If no head object tag is found.</exception>
		/// <exception cref="TypeLoadException">If the type name in the XML is not recognized.</exception>
		public static Type GetTypeFromXML(string filePath)
		{
			if (File.Exists(filePath))
			{
				if (Path.GetExtension(filePath) == ".xml")
				{
					XmlDocument doc = new();
					doc.Load(filePath);
					string typeName = doc.DocumentElement?.LocalName ?? throw new NullReferenceException("No head object tag found!");
					return typeName switch
					{
						#region Items
						"Weapon" => typeof(Weapon),
						"HeadPiece" => typeof(HeadPiece),
						"TorsoPiece" => typeof(TorsoPiece),
						"Aid" => typeof(Aid),
						"Ammo" => typeof(Ammo),
						"Misc" => typeof(Misc),
						#endregion
						#region Entities
						"Player" => typeof(Player),
						"Human" => typeof(Human),
						"Robot" => typeof(Robot),
						#region Mutants
						"Ghoul" => typeof(Ghoul),
						"Feral" => typeof(Feral),
						"SuperMutant" => typeof(SuperMutant),
						"Nightkin" => typeof(Nightkin),
						#endregion
						#region Creatures
						"Dog" => typeof(Dog),
						"NightStalker" => typeof(NightStalker),
						"BloatFly" => typeof(BloatFly),
						"DeathClaw" => typeof(DeathClaw),
						#endregion
						#endregion
						_ => throw new TypeLoadException($"Unknown type: {typeName}")
					};
				}
				throw new FormatException("File is not '*.xml'!");
			}
			throw new FileNotFoundException("File not found.", filePath);
		}

		/// <summary>
		/// Returns the corresponding Type for a given serializable item/entity name.
		/// </summary>
		/// <param name="typeName">The name of the type (e.g., "Weapon", "Player", "Ghoul").</param>
		/// <returns>The Type if found; throws exception if not found.</returns>
		/// <exception cref="SerializationException">If the type is not found.</exception>
		public static Type GetSerializableTypeByName(string typeName)
		{
			// List of serializable base types
			Type[] baseTypes = [
				typeof(Item),
				typeof(Entity),
				typeof(Perk),
				typeof(Location),
				typeof(Quest),
				typeof(Effect),
				typeof(Faction)
			];

			// Gather all non-abstract subclasses for each base type
			foreach (Type baseType in baseTypes)
			{
				// If the base type matches the name
				if (baseType.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase))
				{
					List<Type> subtypes = [.. GetAllSubtypesOf(baseType)];
					if (subtypes.Count == 0)
						Console.WriteLine($"No non-abstract subclasses found for '{baseType.Name}'.");
					else
					{
						Console.WriteLine($"Subclasses of {baseType.Name}:");
						foreach (Type subtype in subtypes)
							Console.WriteLine($"- {subtype.FullName}");
					}
					return baseType;
				}

				// If a subclass matches the name
				List<Type> subtypesList = [.. GetAllSubtypesOf(baseType)];
				Type? subtypeMatch = subtypesList.FirstOrDefault(t => t.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));
				if (subtypeMatch != null)
				{
					Console.WriteLine($"Subclasses of {baseType.Name}:");
					foreach (Type subtype in subtypesList)
						Console.WriteLine($"- {subtype.FullName}");
					return subtypeMatch;
				}
			}

			throw new SerializationException($"Type '{typeName}' not found among serializable base types or their subclasses.");
		}

		/// <summary>
		/// Gets all non-abstract subclasses of the specified base type from its assembly.
		/// </summary>
		/// <param name="baseType">The base type to find subclasses of.</param>
		/// <returns>An enumerable of all non-abstract subclasses of <paramref name="baseType"/>.</returns>
		public static IEnumerable<Type> GetAllSubtypesOf(Type baseType) => Assembly.GetAssembly(baseType).GetTypes()
				.Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(baseType));
	}
}
