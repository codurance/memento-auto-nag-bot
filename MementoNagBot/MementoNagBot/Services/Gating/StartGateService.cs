using MementoNagBot.Models.Misc;
using MementoNagBot.Providers.DateTimes;

namespace MementoNagBot.Services.Gating;

public class StartGateService
{
	private readonly IDateProvider _dateProvider;

	public StartGateService(IDateProvider dateProvider)
	{
		_dateProvider = dateProvider;
	}

	public CanRunResult CanRun()
	{
		DateOnly today = _dateProvider.Today();

		if (today.DayOfWeek is DayOfWeek.Friday)
		{
			// If today is the last working day, we reminded them yesterday
			return IsLastWorkingDayOfMonth(today) ? CanRunResult.CantRun : CanRunResult.CanRunFriday;
		}
		
		// "Tomorrow" in this context pretends weekends don't exist
		int tomorrowOffset = today.DayOfWeek is DayOfWeek.Friday ? 3 : 1;
		DateOnly tomorrow = today.AddDays(tomorrowOffset);

		if (IsLastWorkingDayOfMonth(tomorrow))
		{
			return CanRunResult.CanRunTomorrowLastDay;
		}

		return CanRunResult.CantRun;
	}

	private static bool IsLastWorkingDayOfMonth(DateOnly date)
	{
		int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);

		DateOnly lastDay = new(date.Year, date.Month, daysInMonth);

		return lastDay.DayOfWeek switch
		{
			DayOfWeek.Saturday => date == lastDay.AddDays(-1),
			DayOfWeek.Sunday => date == lastDay.AddDays(-2),
			_ => date == lastDay
		};
	}
}