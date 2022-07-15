using MementoNagBot.Converters;

namespace MementoNagBot.Providers.DateTimes;

public interface IDateProvider
{
	public DateOnly Today();
}