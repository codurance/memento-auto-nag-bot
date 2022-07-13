using System.Text.Json.Serialization;

namespace MementoNagBot.Models.Memento;

// public record MementoUser
// (
// 	string Id,
// 	string Name,
// 	string Email,
// 	MementoRole Role,
// 	bool Active,
// 	string HolidaysRegion
// );

public record MementoUser
{
	[JsonPropertyName("id")]
	public string Id { get; init; } 
	
	[JsonPropertyName("name")]
	public string Name { get; init; } 
	
	[JsonPropertyName("email")]
	public string Email { get; init; }
	
	[JsonPropertyName("role")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public MementoRole Role { get; init; } 
	
	[JsonPropertyName("active")]
	public bool Active { get; init; } 
	
	[JsonPropertyName("holidays_region")]
	public string HolidaysRegion { get; init; } 
}