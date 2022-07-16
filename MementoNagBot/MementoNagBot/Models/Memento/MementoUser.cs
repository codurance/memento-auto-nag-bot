using System.Text.Json.Serialization;

namespace MementoNagBot.Models.Memento;

public record MementoUser
(
	[property: JsonPropertyName("id")]
	string Id,
	
	[property: JsonPropertyName("name")]
	string Name,
	
	[property: JsonPropertyName("email")]
	string Email,
	
	[property: JsonPropertyName("role")]
	[property: JsonConverter(typeof(JsonStringEnumConverter))]
	MementoRole Role,
	
	[property: JsonPropertyName("active")]
	bool Active,
	
	[property: JsonPropertyName("holidays_region")]
	string HolidaysRegion
);