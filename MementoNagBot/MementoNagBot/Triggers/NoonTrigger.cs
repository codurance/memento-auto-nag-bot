using MementoNagBot.Models.Misc;
using MementoNagBot.Services.Gating;
using MementoNagBot.Services.Reminders;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace MementoNagBot.Triggers;

public class NoonTrigger
{
	public const string ScheduleExpression = "0 12 * * MON-FRI";
	
	private readonly StartGateService _startGate;
	private readonly MementoReminderService _reminderService;
	private readonly ILogger<NoonTrigger> _logger;

	public NoonTrigger(StartGateService startGate, MementoReminderService reminderService, ILogger<NoonTrigger> logger)
	{
		_startGate = startGate;
		_reminderService = reminderService;
		_logger = logger;
	}
	
	[FunctionName("NoonTrigger")]
	public async Task RunAsync([TimerTrigger("00 12 * * MON-FRI", RunOnStartup = true)] TimerInfo myTimer, ILogger log)
	{
		_logger.LogInformation("Function Run with Noon Trigger");
		
		CanRunResult canRunResult = _startGate.CanRun();
		
		switch (canRunResult)
		{
			case CanRunResult.CanRunTomorrowLastDay:
				await _reminderService.SendGeneralReminder(true);
				return;
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