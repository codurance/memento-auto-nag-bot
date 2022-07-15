using MementoNagBot.Services.Messaging;

namespace MementoNagBot.Services.Reminders;

public class MementoReminderService
{
	private readonly SlackMessageService _messageService;

	private const string GeneralReminderText = "Hi everyone, it's that time of week again, could you please make sure your Memento is up to date!";
	private const string MonthEndReminderText = "Hi everyone, tomorrow is month end, could you please make sure that your Memento is up to date and remember to fill it out for tomorrow too!";
	private const string IndividualReminderTemplate = "Hi {0}, it looks like you've forgotten to fill out your Memento. We know you're busy but we really need it to be kept up to date for billing purposes. So please could you ensure it's updated as soon as possible! If you're having difficulties doing so, please reach out to your manager!";

	public MementoReminderService(SlackMessageService messageService)
	{
		_messageService = messageService;
	}

	public async Task SendGeneralReminder(bool tomorrowIsLastDay) => await _messageService.SendMessageToBotChannel(tomorrowIsLastDay ? MonthEndReminderText : GeneralReminderText);

	public async Task SendIndividualReminders(bool tomorrowIsLastDay)
	{
		
	}
}