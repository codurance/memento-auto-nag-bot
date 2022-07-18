using System.Collections;
using System.Collections.Generic;

namespace MementoNagBot.Models.Misc;

public record InclusiveDateRange(DateOnly StartDate, DateOnly EndDate): IEnumerable<DateOnly>
{
	public readonly int TotalDays = EndDate.DayNumber - StartDate.DayNumber + 1;
	
	

	public List<DateOnly> GetDaysInRange()
	{
		List<DateOnly> dates = new();
		DateOnly date = StartDate;
		do
		{
			dates.Add(date);
			date = date.AddDays(1);
		} while (date <= EndDate);

		return dates;
	}

	public IEnumerator<DateOnly> GetEnumerator() => GetDaysInRange().GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}