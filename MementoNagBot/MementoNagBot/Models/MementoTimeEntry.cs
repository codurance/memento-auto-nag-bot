using System;

namespace MementoNagBot.Models;

public record MementoTimeEntry
(
	Guid Id,
	Guid ActivityId,
	string UserId,
	string Title,
	TimeOnly StartTime,
	TimeOnly FinishTime,
	DateOnly Start,
	DateOnly ActivityDate,
	int Hours
);