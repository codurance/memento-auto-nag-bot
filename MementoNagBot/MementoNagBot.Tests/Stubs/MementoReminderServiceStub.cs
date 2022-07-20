using MementoNagBot.Services.Reminders;

namespace MementoNagBot.Tests.Stubs;

public class MementoReminderServiceStub: MementoReminderService
{
	public MementoReminderServiceStub(): base(null!, null!, null!, null!, null!, null!) {}

	public bool SentGeneralReminderFriday { get; private set; }
	public bool SentGeneralReminderMonthEnd { get; private set; }
	public bool SentIndividualReminderFriday { get; private set; }
	public bool SentIndividualReminderMonthEnd { get; private set; }
	
	public override Task SendGeneralReminder(bool tomorrowIsLastDay)
	{
		if (tomorrowIsLastDay)
		{
			SentGeneralReminderMonthEnd = true;
		}
		else
		{
			SentGeneralReminderFriday = true;
		}
		
		
		return Task.CompletedTask;
	}

	public override Task SendIndividualReminders(bool tomorrowIsLastDay)
	{
		if (tomorrowIsLastDay)
		{
			SentIndividualReminderMonthEnd = true;
		}
		else
		{
			SentIndividualReminderFriday = true;
		}
		
		
		return Task.CompletedTask;
	}
}