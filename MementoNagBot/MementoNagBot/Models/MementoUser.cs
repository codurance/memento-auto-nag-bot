namespace MementoNagBot.Models;

public record MementoUser
(
	string Id,
	string Name,
	string Email,
	MementoRole Role,
	bool Active,
	string HolidaysRegion
);