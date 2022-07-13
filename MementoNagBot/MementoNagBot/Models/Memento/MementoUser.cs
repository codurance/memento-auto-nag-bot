namespace MementoNagBot.Models.Memento;

public record MementoUser
(
	string Id,
	string Name,
	string Email,
	MementoRole Role,
	bool Active,
	string HolidaysRegion
);