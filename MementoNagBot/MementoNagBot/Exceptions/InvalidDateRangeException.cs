namespace MementoNagBot.Exceptions;

public class InvalidDateRangeException: Exception
{
	public DateOnly StartDate { get; }
	public DateOnly EndDate { get; }

	public InvalidDateRangeException(DateOnly startDate, DateOnly endDate): base("Start date ({StartDate}) must be before end date ({EndDate}), you're not a theoretical physicist")
	{
		StartDate = startDate;
		EndDate = endDate;
	}
}