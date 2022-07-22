using MementoNagBot.Models.Misc;
using MementoNagBot.Services.Gating;
using MementoNagBot.Services.Reminders;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace MementoNagBot.Triggers;

public class AfternoonTrigger
{
	public const string ScheduleExpression = "0 16 * * MON-FRI";
	
	private readonly StartGateService _startGate;
	private readonly MementoReminderService _reminderService;
	private readonly ILogger<AfternoonTrigger> _logger;

	public AfternoonTrigger(StartGateService startGate, MementoReminderService reminderService, ILogger<AfternoonTrigger> logger)
	{
		_startGate = startGate;
		_reminderService = reminderService;
		_logger = logger;
	}

	[FunctionName("AfternoonTrigger")]
	public async Task RunAsync([TimerTrigger(ScheduleExpression)] TimerInfo myTimer, ILogger log)
	{
		_logger.LogInformation("Function Run with Afternoon Timed Trigger");
		
		CanRunResult canRunResult = _startGate.CanRun();
		
		switch (canRunResult)
		{
			case CanRunResult.CanRunTomorrowLastDay: 
				await _reminderService.SendIndividualReminders(true);
				return;
			case CanRunResult.CanRunFriday:
				await _reminderService.SendIndividualReminders(false);
				return;
			case CanRunResult.CantRun:
				return;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
}