using MementoNagBot.Models.Misc;
using MementoNagBot.Services.Gating;
using MementoNagBot.Services.Reminders;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace MementoNagBot.Triggers;

public class NoonTrigger
{
	private readonly StartGateService _startGate;
	private readonly MementoReminderService _reminderService;

	public NoonTrigger(StartGateService startGate, MementoReminderService reminderService)
	{
		_startGate = startGate;
		_reminderService = reminderService;
	}
	
	[FunctionName("NoonTrigger")]
	public async Task RunAsync([TimerTrigger("0 0 12 * * MON-FRI")] TimerInfo myTimer, ILogger log)
	{
		CanRunResult canRunResult = _startGate.CanRun();
		
		switch (canRunResult)
		{
			case CanRunResult.CanRunTomorrowLastDay:
				await _reminderService.SendGeneralReminder(true);
				break;
			case CanRunResult.CanRunFriday:
				await _reminderService.SendGeneralReminder(false);
				return;
			case CanRunResult.CantRun:
				return;
			default:
				throw new ArgumentOutOfRangeException();
		}
		
	}
}