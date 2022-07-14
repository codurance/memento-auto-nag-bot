using System.Text.Json.Serialization;
using MementoNagBot.Converters;

namespace MementoNagBot.Models.Memento;

public record MementoTimeEntry
(
	Guid Id,
	Guid ActivityId,
	string UserId,
	string Title,
	TimeOnly StartTime,
	TimeOnly FinishTime,
	[property: JsonConverter(typeof(DateOnlyConverter))]
	DateOnly Start,
	[property: JsonConverter(typeof(DateOnlyConverter))]
	DateOnly ActivityDate,
	int Hours
);