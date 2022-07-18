using System.Collections;
using System.Collections.Generic;

namespace MementoNagBot.Models.Misc;

public record InclusiveDateRange(DateOnly StartDate, DateOnly EndDate): IEnumerable<DateOnly>
{
	public readonly int TotalDays = EndDate.DayNumber - StartDate.DayNumber + 1;

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