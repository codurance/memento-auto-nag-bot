using MementoNagBot.Models.Misc;
using MementoNagBot.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace MementoNagBot.Triggers;

public class AfternoonTrigger
{
	private readonly StartGateService _startGate;

	public AfternoonTrigger(StartGateService startGate)
	{
		_startGate = startGate;
	}

	[FunctionName("AfternoonTrigger")]
	public async Task RunAsync([TimerTrigger("0 40 17 * * MON-FRI")] TimerInfo myTimer, ILogger log)
	{
		CanRunResult canRunResult = _startGate.CanRun();
		
		switch (canRunResult)
		{
			case CanRunResult.CanRunTomorrowLastDay:
				throw new NotImplementedException();
			case CanRunResult.CanRunFriday:
				throw new NotImplementedException();
			case CanRunResult.CantRun:
				return;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
}