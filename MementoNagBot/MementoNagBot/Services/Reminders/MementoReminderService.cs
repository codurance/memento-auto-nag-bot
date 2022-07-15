using System.Collections.Generic;
using System.Linq;
using MementoNagBot.Clients.Memento;
using MementoNagBot.Models.Memento;
using MementoNagBot.Models.Misc;
using MementoNagBot.Providers.DateTimes;
using MementoNagBot.Services.Messaging;

namespace MementoNagBot.Services.Reminders;

public class MementoReminderService
{
	private readonly SlackMessageService _messageService;
	private readonly IMementoClient _mementoClient;
	private readonly IDateProvider _dateProvider;


	//TODO - Making these public for now, as when I add the translation service, they'll be injected. Just makes this iteration easier.
	public const string GeneralReminderText = "Hi everyone, it's that time of week again, could you please make sure your Memento is up to date!";
	public const string MonthEndReminderText = "Hi everyone, tomorrow is month end, could you please make sure that your Memento is up to date and remember to fill it out for tomorrow too!";
	public const string IndividualReminderTemplate = "Hi {0}, it looks like you've forgotten to fill out your Memento. We know you're busy but we really need it to be kept up to date for billing purposes. So please could you ensure it's updated as soon as possible! If you're having difficulties doing so, please reach out to your manager!";
	public const string MonthEndIndividualReminderTemplate = "Hi {0}, it looks like you've forgotten to fill out your Memento. We know you're busy but we really need it to be kept up to date for billing purposes. So please could you ensure it's updated as soon as possible! Please also remember to fill out tomorrow as it's month end. If you're having difficulties doing so, please reach out to your manager!";

	public MementoReminderService(SlackMessageService messageService, IMementoClient mementoClient, IDateProvider dateProvider)
	{
		_messageService = messageService;
		_mementoClient = mementoClient;
		_dateProvider = dateProvider;
	}

	public async Task SendGeneralReminder(bool tomorrowIsLastDay) => await _messageService.SendMessageToBotChannel(tomorrowIsLastDay ? MonthEndReminderText : GeneralReminderText);
	
	public async Task SendIndividualReminders(bool tomorrowIsLastDay)
	{
		InclusiveDateRange dateRange = GetRelevantDates(tomorrowIsLastDay);
		
		// It would be great if we could request time entries for all users, but this endpoint doesn't exist on memento.
		// There is the summary API which contains this information, but it's in an awkward format to parse out what we need.
		// We also don't really care about the number of API calls this will make since Memento isn't exactly high-traffic and it only runs once a week.
		// I think this is the cognitively easiest solution, despite its inefficiency, so I've gone with it.
		// I also don't know lisp... Or I'd add the endpoint to Memento.
		// If this ever happens, use this endpoint instead: https://github.com/codurance/memento/issues/62

		List<MementoUser> users = await _mementoClient.GetActiveInternalUsers();

		foreach (MementoUser user in users)
		{
			List<MementoTimeEntry> timeEntries = await _mementoClient.GetTimeEntriesForUser(user.Email, dateRange);
			
			bool fullTimeSheet = timeEntries.Sum(te => te.Hours) >= dateRange.TotalDays * 8;
			if (fullTimeSheet) continue;
			
			string template = tomorrowIsLastDay ? MonthEndIndividualReminderTemplate : IndividualReminderTemplate;
			string message = string.Format(template, user.Name.Split(' ')[0]);
			await _messageService.SendDirectMessageToUser(user.Email, message);
		}
	}

	private InclusiveDateRange GetRelevantDates(bool tomorrowIsLastDay)
	{
		DateOnly endDate = tomorrowIsLastDay ? _dateProvider.Today().AddDays(1) : _dateProvider.Today();

		DateOnly startDate = endDate.DayOfWeek switch
		{
			DayOfWeek.Monday => endDate,
			DayOfWeek.Tuesday => endDate.AddDays(-1),
			DayOfWeek.Wednesday => endDate.AddDays(-2),
			DayOfWeek.Thursday => endDate.AddDays(-3),
			DayOfWeek.Friday => endDate.AddDays(-4),
			DayOfWeek.Saturday => throw new ArgumentOutOfRangeException(),
			DayOfWeek.Sunday => throw new ArgumentOutOfRangeException(),
			_ => throw new ArgumentOutOfRangeException()
		};

		return new(startDate, endDate);
	}
}