using System;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Weather
{
	/// <summary>
	/// Represents weather data from the OpenWeatherMap API, including coordinates, weather conditions, temperature, wind, clouds, and more.
	/// </summary>
	public class WeatherData
	{
		/// <summary>
		/// The geographical coordinates (longitude and latitude) of the location.
		/// </summary>
		[JsonPropertyName("coord")]
		[JsonConverter(typeof(Vector2Converter))]
		public Vector2 Coordinates { get; set; } = new();

		/// <summary>
		/// Represents a single weather condition.
		/// </summary>
		public class Weather
		{
			/// <summary>
			/// The weather condition ID from the API.
			/// </summary>
			[JsonPropertyName("id")]
			public int Id { get; set; }

			/// <summary>
			/// The main weather category (e.g., "Rain", "Clear", "Clouds").
			/// </summary>
			[JsonPropertyName("main")]
			public string Main { get; set; } = string.Empty;

			/// <summary>
			/// A detailed description of the weather condition.
			/// </summary>
			[JsonPropertyName("description")]
			public string Description { get; set; } = string.Empty;

			/// <summary>
			/// Gets an emoji icon representing the weather condition based on the weather ID.
			/// </summary>
			public string Icon => Id.ToString()[0] switch
			{
				'2' => "⛈️",
				'3' => "🌧️",
				'5' => "🌦️",
				'6' => "🌨️",
				'7' => "🌫️",
				_ => "☀️/🌕"
			};

			/// <inheritdoc/>
			public override string ToString() => $"Weather: {Main} ({Description}), Icon: {Icon}";
		}

		/// <summary>
		/// An array of weather conditions at this location.
		/// </summary>
		[JsonPropertyName("weather")]
		public Weather[] Weathers { get; set; } = [];

		/// <summary>
		/// The base station (internal parameter from API).
		/// </summary>
		[JsonPropertyName("base")]
		public string Base { get; set; } = string.Empty;

		/// <summary>
		/// Represents main weather measurements including temperature, pressure, and humidity.
		/// </summary>
		public class Main
		{
			[JsonPropertyName("temp")]
			public float Temperature { get; set; }

			[JsonPropertyName("feels_like")]
			public float FeelsLike { get; set; }

			[JsonPropertyName("temp_min")]
			public float MinTemperature { get; set; }

			[JsonPropertyName("temp_max")]
			public float MaxTemperature { get; set; }

			[JsonPropertyName("pressure")]
			public int Pressure { get; set; }

			[JsonPropertyName("humidity")]
			public int Humidity { get; set; }

			[JsonPropertyName("sea_level")]
			public int SeaLevel { get; set; }

			[JsonPropertyName("grnd_level")]
			public int GroundLevel { get; set; }

			public override string ToString() =>
				$"Temperature: {Temperature}°F (Feels like: {FeelsLike}°F), Min: {MinTemperature}°F, Max: {MaxTemperature}°F" + Environment.NewLine +
				$"Pressure: {Pressure} hPa, Humidity: {Humidity}%" + Environment.NewLine +
				$"Sea Level: {SeaLevel} hPa, Ground Level: {GroundLevel} hPa";
		}

		/// <summary>
		/// Main weather data including temperature, pressure, and humidity measurements.
		/// </summary>
		[JsonPropertyName("main")]
		public Main MainData { get; set; } = new();

		/// <summary>
		/// Visibility in meters.
		/// </summary>
		[JsonPropertyName("visibility")]
		public int Visibility { get; set; }

		/// <summary>
		/// Represents wind data including speed and direction.
		/// </summary>
		public class Wind
		{
			[JsonPropertyName("speed")]
			public float Speed { get; set; }

			[JsonPropertyName("deg")]
			public int Deg { get; set; }

			public override string ToString() =>
				$"Speed: {Speed} m/s, Direction: {Deg}°" + (Speed > 30 ? "💨" : string.Empty);
		}

		/// <summary>
		/// Wind data for this location.
		/// </summary>
		[JsonPropertyName("wind")]
		public Wind WindData { get; set; } = new();

		/// <summary>
		/// Represents cloud coverage data.
		/// </summary>
		public class Clouds
		{
			/// <summary>
			/// Cloudiness percentage (0-100).
			/// </summary>
			[JsonPropertyName("all")]
			public int Cloudiness { get; set; }

			/// <summary>
			/// Gets an emoji icon representing the cloud coverage level.
			/// </summary>
			public string CloudIcon => Cloudiness switch
			{
				< 10 => "☀️",
				< 25 => "🌤️",
				< 50 => "⛅",
				_ => "☁️"
			};

			public override string ToString() => $"Cloudiness: {Cloudiness}%" + CloudIcon;
		}

		/// <summary>
		/// Cloud coverage data for this location.
		/// </summary>
		[JsonPropertyName("clouds")]
		public Clouds CloudData { get; set; } = new();

		/// <summary>
		/// The timestamp of when the data was calculated (Unix timestamp converted to DateTime).
		/// </summary>
		[JsonPropertyName("dt")]
		[JsonConverter(typeof(UnixDateTimeConverter))]
		public DateTime DT { get; set; } = new();

		/// <summary>
		/// Represents system-level data including country code, sunrise, and sunset times.
		/// </summary>
		public class Sys
		{
			[JsonPropertyName("type")]
			public int Type { get; set; }

			[JsonPropertyName("id")]
			public int Id { get; set; }

			[JsonPropertyName("country")]
			public string Country { get; set; } = string.Empty;

			[JsonPropertyName("sunrise")]
			[JsonConverter(typeof(UnixDateTimeConverter))]
			public DateTime Sunrise { get; set; } = new();

			[JsonPropertyName("sunset")]
			[JsonConverter(typeof(UnixDateTimeConverter))]
			public DateTime Sunset { get; set; } = new();

			public override string ToString() =>
				$"Type: {Type}, ID: {Id}, Country: {Country}, Sunrise: {Sunrise}, Sunset: {Sunset}";
		}

		/// <summary>
		/// System data including country information and sunrise/sunset times.
		/// </summary>
		[JsonPropertyName("sys")]
		public Sys SysData { get; set; } = new();

		/// <summary>
		/// Timezone offset from UTC in seconds.
		/// </summary>
		[JsonPropertyName("timezone")]
		public int Timezone { get; set; }

		/// <summary>
		/// City ID from the API.
		/// </summary>
		[JsonPropertyName("id")]
		public int Id { get; set; }

		/// <summary>
		/// City name.
		/// </summary>
		[JsonPropertyName("name")]
		public string Name { get; set; } = string.Empty;

		/// <summary>
		/// Internal response code from the API.
		/// </summary>
		[JsonPropertyName("cod")]
		public int Code { get; set; }

		public override string ToString() =>
			$"Location: {Name}, Coordinates: ({Coordinates.X}, {Coordinates.Y})" + Environment.NewLine +
			$"{string.Join(Environment.NewLine, (object[])Weathers)}" + Environment.NewLine +
			$"Base: {Base}" + Environment.NewLine +
			$"{MainData}" + Environment.NewLine +
			$"Visibility: {Visibility} meters" + Environment.NewLine +
			$"{WindData}" + Environment.NewLine +
			$"{CloudData}" + Environment.NewLine +
			$"Date/Time: {DT}" + Environment.NewLine +
			$"{SysData}" + Environment.NewLine +
			$"Timezone: {Timezone}, ID: {Id}, Code: {Code}";

		/// <summary>
		/// Returns a shortened version of the weather data showing only temperature, wind speed, and icon.
		/// </summary>
		/// <param name="shorthand">Unused parameter retained for method signature compatibility.</param>
		/// <returns>A brief summary of the weather data.</returns>
		public string ToString(bool shorthand) =>
			MainData.Temperature + "°F, " + Environment.NewLine +
			WindData.Speed + "m/s" + Environment.NewLine +
			Weathers[0].Icon;

		/// <summary>
		/// Custom JSON converter for converting Unix timestamps to <see cref="DateTime"/> objects.
		/// </summary>
		public class UnixDateTimeConverter : JsonConverter<DateTime>
		{
			/// <inheritdoc/>
			public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				return DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64()).DateTime;
			}

			/// <inheritdoc/>
			public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
			{
				writer.WriteNumberValue(((DateTimeOffset)value).ToUnixTimeSeconds());
			}

		}

		/// <summary>
		/// Custom JSON converter for converting coordinate objects to <see cref="Vector2"/> objects.
		/// </summary>
		public class Vector2Converter : JsonConverter<Vector2>
		{
			/// <inheritdoc/>
			public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				if (reader.TokenType != JsonTokenType.StartObject)
					throw new JsonException();

				float lon = 0;
				float lat = 0;

				while (reader.Read())
				{
					if (reader.TokenType == JsonTokenType.EndObject)
						return new Vector2(lon, lat);

					if (reader.TokenType == JsonTokenType.PropertyName)
					{
						string propertyName = reader.GetString();
						reader.Read();

						switch (propertyName)
						{
							case "lon":
								lon = reader.GetSingle();
								break;
							case "lat":
								lat = reader.GetSingle();
								break;
						}
					}
				}

				throw new JsonException();
			}

			/// <inheritdoc/>
			public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
			{
				writer.WriteStartObject();
				writer.WriteNumber("lon", value.X);
				writer.WriteNumber("lat", value.Y);
				writer.WriteEndObject();
			}
		}
	}
}