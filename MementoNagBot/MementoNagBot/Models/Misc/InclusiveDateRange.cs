namespace MementoNagBot.Models.Misc;

public record InclusiveDateRange(DateOnly StartDate, DateOnly EndDate)
{
	public readonly int TotalDays = EndDate.DayNumber - StartDate.DayNumber + 1;
}