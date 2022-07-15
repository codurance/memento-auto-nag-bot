namespace MementoNagBot.Providers.DateTimes;

public class SystemDateProvider: IDateProvider
{
	public DateOnly Today() => DateOnly.FromDateTime(DateTime.Now.Date);
}