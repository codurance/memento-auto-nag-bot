using System.Collections;
using System.Collections.Generic;
using MementoNagBot.Exceptions;

namespace MementoNagBot.Models.Misc;

public record InclusiveDateRange: IEnumerable<DateOnly>
{
	public DateOnly StartDate { get; }
	public DateOnly EndDate { get; }
	public int TotalDays { get; }

	public InclusiveDateRange(DateOnly startDate, DateOnly endDate)
	{
		if (startDate > endDate) throw new InvalidDateRangeException(startDate, endDate);

		StartDate = startDate;
		EndDate = endDate;
		TotalDays = EndDate.DayNumber - StartDate.DayNumber + 1;
	}

	public IEnumerator<DateOnly> GetEnumerator()
	{
		DateOnly date = StartDate;
		do
		{
			yield return date;
			date = date.AddDays(1);
		} while (date <= EndDate);
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}