using AutoFixture;
using MementoNagBot.Models.Memento;
using MementoNagBot.Models.Misc;

namespace MementoNagBot.Tests.TestDataGenerators;

public static class TimeSheetTestGenerator
{
	public static IEnumerable<object[]> GetFullTimeSheets(int number)
	{
		Fixture fixture = new();
		fixture.Customize<DateOnly>(composer => composer.FromFactory<DateTime>(DateOnly.FromDateTime));

		for (int i = 0; i < number; i++)
		{
			int hoursInDay = fixture.Create<int>();
			DateOnly endDate = fixture.Create<DateOnly>();
			DateOnly startDate = GetWeekStart(endDate);
			InclusiveDateRange range = new(startDate, endDate);

			MementoTimeSheet timeSheet = new(range, null, hoursInDay);

			foreach (DateOnly date in range)
			{
				int totalHours = 0;

				while (totalHours < hoursInDay)
				{
					MementoTimeEntry entry = fixture.Create<MementoTimeEntry>()
						with
						{
							Hours = fixture.Create<int>() % hoursInDay,
							ActivityDate = date
						};
					totalHours += entry.Hours;
					timeSheet.Add(entry);
				}

			}
			
			yield return new object[] {timeSheet};
		}
	}
	
	public static IEnumerable<object[]> GetIncompleteTimeSheets(int number)
	{
		Fixture fixture = new();
		fixture.Customize<DateOnly>(composer => composer.FromFactory<DateTime>(DateOnly.FromDateTime));

		for (int i = 0; i < number; i++)
		{
			int hoursInDay = fixture.Create<int>();
			DateOnly endDate = fixture.Create<DateOnly>();
			DateOnly startDate = GetWeekStart(endDate);
			InclusiveDateRange range = new(startDate, endDate);

			MementoTimeSheet timeSheet = new(range, null, hoursInDay);

			foreach (DateOnly date in range)
			{
				MementoTimeEntry entry = fixture.Create<MementoTimeEntry>()
						with
						{
							Hours = fixture.Create<int>() % hoursInDay - 2, // Will never be complete
							ActivityDate = date
						};
				timeSheet.Add(entry);
			}
			
			yield return new object[] {timeSheet};
		}
	}

	private static DateOnly GetWeekStart(DateOnly date) =>
		// Can't do simple -6 + (int)DayOfWeek here because Americans, in their wisdom, think Sunday is the start of a week...
		date.DayOfWeek switch
		{
			DayOfWeek.Monday => date,
			DayOfWeek.Tuesday => date.AddDays(-1),
			DayOfWeek.Wednesday => date.AddDays(-2),
			DayOfWeek.Thursday => date.AddDays(-3),
			DayOfWeek.Friday => date.AddDays(-4),
			DayOfWeek.Saturday => date.AddDays(-5),
			DayOfWeek.Sunday => date.AddDays(-6),
			_ => throw new ArgumentOutOfRangeException()
		};
}