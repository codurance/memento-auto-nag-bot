using MementoNagBot.Models.Misc;
using MementoNagBot.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace MementoNagBot.Triggers;

public class NoonTrigger
{
	private readonly StartGateService _startGate;

	public NoonTrigger(StartGateService startGate)
	{
		_startGate = startGate;
	}
	
	[FunctionName("NoonTrigger")]
	public async Task RunAsync([TimerTrigger("0 0 12 * * MON-FRI")] TimerInfo myTimer, ILogger log)
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