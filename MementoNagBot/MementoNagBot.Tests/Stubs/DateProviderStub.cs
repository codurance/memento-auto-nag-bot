using MementoNagBot.Providers.DateTimes;

namespace MementoNagBot.Tests.Stubs;

public class DateProviderStub: IDateProvider
{
	private readonly DateOnly _retVal;

	public DateProviderStub(DateOnly retVal)
	{
		_retVal = retVal;
	}

	public DateOnly Today() => _retVal;
}