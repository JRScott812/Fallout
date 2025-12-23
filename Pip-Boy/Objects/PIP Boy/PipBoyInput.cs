using System;
using System.Linq;
using System.Reflection;

namespace Pip_Boy.Objects.PIP_Boy
{
	/// <summary>
	/// Handles all user input processing for the PIP-Boy.
	/// </summary>
	/// <remarks>
	/// Initializes a new instance of the <see cref="PipBoyInput"/> class.
	/// </remarks>
	/// <param name="pipBoy">Reference to the parent PipBoy instance.</param>
	/// <param name="audio">Reference to the audio manager.</param>
	public class PipBoyInput(PipBoy pipBoy, PipBoyAudio audio)
	{
		private readonly PipBoy _pipBoy = pipBoy;
		private readonly PipBoyAudio _audio = audio;

		#region Menu Navigation
		/// <summary>
		/// Changes the current menu page.
		/// </summary>
		/// <param name="right">Move right?</param>
		/// <param name="currentPage">Reference to the current page</param>
		public void ChangeMenu(bool right, ref PipBoy.Pages currentPage)
		{
			_audio.PlayMenuSound();
			if (right && currentPage < PipBoy.Pages.DATA)
			{
				currentPage++;
			}
			if (!right && currentPage > PipBoy.Pages.STATS)
			{
				currentPage--;
			}
		}

		/// <summary>
		/// Changes the current sub menu page.
		/// </summary>
		/// <param name="right">Move right?</param>
		/// <param name="currentPage">The current main page</param>
		/// <param name="statPage">Reference to the current STAT sub-page</param>
		/// <param name="dataPage">Reference to the current DATA sub-page</param>
		public void ChangeSubMenu(bool right, PipBoy.Pages currentPage, ref PipBoy.StatsPages statPage, ref PipBoy.DataPages dataPage)
		{
			_audio.PlayMenuSound();

			switch (currentPage)
			{
				case PipBoy.Pages.STATS when right && statPage < PipBoy.StatsPages.General:
					statPage++;
					break;
				case PipBoy.Pages.STATS when !right && statPage > PipBoy.StatsPages.Status:
					statPage--;
					break;

				case PipBoy.Pages.ITEMS when right && _pipBoy.Player.Inventory.itemPage < Inventory.ItemsPages.Misc:
					_pipBoy.Player.Inventory.itemPage++;
					break;
				case PipBoy.Pages.ITEMS when !right && _pipBoy.Player.Inventory.itemPage > Inventory.ItemsPages.Weapons:
					_pipBoy.Player.Inventory.itemPage--;
					break;

				case PipBoy.Pages.DATA when right && dataPage < PipBoy.DataPages.Radio:
					dataPage++;
					break;
				case PipBoy.Pages.DATA when !right && dataPage > PipBoy.DataPages.Map:
					dataPage--;
					break;
			}
		}

		/// <summary>
		/// Changes the selected faction, in order to show their description.
		/// </summary>
		/// <param name="up">Move up the list</param>
		public void ChangeSelectedFaction(bool up)
		{
			if (!up && _pipBoy.FactionIndex > 0)
			{
				_pipBoy.FactionIndex--;
				_audio.PlayFactionDownSound();
			}

			if (up && _pipBoy.FactionIndex < _pipBoy.Factions.Length - 1)
			{
				_pipBoy.FactionIndex++;
				_audio.PlayFactionUpSound();
			}
		}
		#endregion

		#region Input Utilities
		/// <summary>
		/// Prompts the user to enter a value of the specified type.
		/// </summary>
		/// <typeparam name="T">The type of the value to be entered.</typeparam>
		/// <param name="message">The message to display before the input prompt.</param>
		/// <returns>The entered value converted to the specified type.</returns>
		public static T EnterValue<T>(string message) where T : IConvertible
		{
			Type type = typeof(T);
			T variable = default;
			bool isValid = false;
			string promptMessage = $"{message} ({type.Name}";

			if (type != typeof(bool) && type != typeof(string))
			{
				T minValue = (T)Convert.ChangeType(type.GetField("MinValue").GetValue(null), typeof(T));
				T maxValue = (T)Convert.ChangeType(type.GetField("MaxValue").GetValue(null), typeof(T));
				promptMessage += $", Min: {minValue}, Max: {maxValue}";
			}
			promptMessage += "): ";

			do
			{
				Console.Write(promptMessage);
				string input = Console.ReadLine();
				try
				{
					if (type == typeof(bool))
					{
						if (bool.TryParse(input, out bool boolResult))
						{
							variable = (T)Convert.ChangeType(boolResult, typeof(T));
							isValid = true;
						}
						else
						{
							Console.Error.WriteLine($"Invalid input. Please enter a valid {type.Name} (true/false).");
						}
					}
					else
					{
						variable = (T)Convert.ChangeType(input, typeof(T));
						isValid = true;
					}
				}
				catch
				{
					Console.Error.WriteLine($"Invalid input. Please enter a valid {type.Name}.");
				}
			} while (!isValid);

			return variable;
		}

		/// <summary>
		/// Creates an instance of the specified <paramref name="type"/> by prompting the user to enter values for each constructor parameter.
		/// </summary>
		/// <param name="type">The type of object to create. Must have a public constructor.</param>
		/// <returns>
		/// An instance of <paramref name="type"/> with its constructor parameters set to the values entered by the user.
		/// </returns>
		public static object CreateFromInput(Type type)
		{
			ArgumentNullException.ThrowIfNull(type);

			// Get the constructor with the most parameters (prefer the most complete one)
			ConstructorInfo ctor = type.GetConstructors()
				.OrderByDescending(c => c.GetParameters().Length)
				.FirstOrDefault() ?? throw new InvalidOperationException($"No public constructor found for type {type.FullName}.");
			ParameterInfo[] parameters = ctor.GetParameters();
			object[] args = new object[parameters.Length];

			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo param = parameters[i];
				Type paramType = param.ParameterType;
				object value;

				// Use EnterValue<T> for primitives and strings, otherwise fallback to recursion for complex types
				if (paramType.IsPrimitive || paramType == typeof(string) || paramType == typeof(decimal))
				{
					MethodInfo enterValueMethod = typeof(PipBoyInput).GetMethod(nameof(EnterValue)).MakeGenericMethod(paramType);
					value = enterValueMethod.Invoke(null, [$"Enter value for {param.Name}"]);
				}
				else if (paramType.IsEnum)
				{
					Console.WriteLine($"Select {param.Name} ({paramType.Name}):");
					string[] names = Enum.GetNames(paramType);
					for (int j = 0; j < names.Length; j++)
						Console.WriteLine($"{j}: {names[j]}");
					int selected = -1;
					while (selected < 0 || selected >= names.Length)
					{
						Console.Write("Enter number: ");
						string input = Console.ReadLine();
						if (int.TryParse(input, out selected) && selected >= 0 && selected < names.Length)
							break;
						Console.WriteLine("Invalid selection.");
					}
					value = Enum.Parse(paramType, names[selected]);
				}
				else
				{
					Console.WriteLine($"Creating value for parameter '{param.Name}' of type '{paramType.Name}'...");
					value = CreateFromInput(paramType);
				}

				args[i] = value;
			}

			return ctor.Invoke(args);
		}
		#endregion
	}
}
