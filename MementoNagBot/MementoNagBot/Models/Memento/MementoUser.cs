using System.Text.Json.Serialization;
using JetBrains.Annotations;
using MementoNagBot.Models.Misc;

namespace MementoNagBot.Models.Memento;

[UsedImplicitly]
public record MementoUser
(
	[property: JsonPropertyName("id")] string Id,

	[property: JsonPropertyName("name")] string Name,

	[property: JsonPropertyName("email")] string Email,

	[property: JsonPropertyName("role")] [property: JsonConverter(typeof(JsonStringEnumConverter))]
	MementoRole Role,

	[property: JsonPropertyName("active")] bool Active,

	[property: JsonPropertyName("holidays_region")]
	string HolidaysRegion
)
{
	public Language GetLanguage() =>
		HolidaysRegion switch
		{
			"UK" => Language.English,
			
			"Aragon" => Language.Spanish,
			"Barcelona" => Language.Spanish,
			"Madrid" => Language.Spanish,
			"Valencia" => Language.Spanish,
			"Andalucia" => Language.Spanish,
			"Sevilla" => Language.Spanish,
			"País Vasco" => Language.Spanish,
			"Galicia" => Language.Spanish,
			"Murcia" => Language.Spanish,
			
			"Lisbon" => Language.Portuguese,
			
			"France" => Language.French,
			
			_ => Language.English
		};
};
