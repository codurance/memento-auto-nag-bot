using System.Collections.Generic;
using System.Linq;
using MementoNagBot.Clients.Memento;
using MementoNagBot.Models.Memento;
using MementoNagBot.Models.Misc;
using MementoNagBot.Models.Options;
using MementoNagBot.Providers.DateTimes;
using MementoNagBot.Services.Messaging;
using MementoNagBot.Services.Translation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MementoNagBot.Services.Reminders;

public class MementoReminderService
{
	private readonly SlackMessageService _messageService;
	private readonly IMementoClient _mementoClient;
	private readonly IDateProvider _dateProvider;
	private readonly ITranslatedResourceService _translatedResourceService;
	private readonly IOptions<MementoOptions> _options;
	private readonly ILogger<MementoReminderService> _logger;
	
	public MementoReminderService(
		SlackMessageService messageService,
		IMementoClient mementoClient, 
		IDateProvider dateProvider,
		ITranslatedResourceService translatedResourceService,
		IOptions<MementoOptions> options,
		ILogger<MementoReminderService> logger
		)
	{
		_messageService = messageService;
		_mementoClient = mementoClient;
		_dateProvider = dateProvider;
		_translatedResourceService = translatedResourceService;
		_options = options;
		_logger = logger;
	}

	public virtual async Task SendGeneralReminder(bool tomorrowIsLastDay)
	{
		_logger.LogInformation("Function Run with Manual Trigger");

		TranslatedResourceCompoundKey key = new(tomorrowIsLastDay ? TranslatedResource.GeneralReminderMonthEndTemplate : TranslatedResource.GeneralReminderTemplate, Language.English);
		string message = _translatedResourceService.GetTranslatedString(key);
		
		await _messageService.SendMessageToBotChannel($"<!channel> {message}");
	}

	public virtual async Task SendIndividualReminders(bool tomorrowIsLastDay)
	{
		_logger.LogInformation("Sending individual reminders because: {Reason}", tomorrowIsLastDay ? "tomorrow is last working day" : "it's Friday");
		
		InclusiveDateRange dateRange = GetRelevantDates(tomorrowIsLastDay);
		
		// It would be great if we could request time entries for all users, but this endpoint doesn't exist on memento.
		// There is the summary API which contains this information, but it's in an awkward format to parse out what we need.
		// We also don't really care about the number of API calls this will make since Memento isn't exactly high-traffic and it only runs once a week.
		// I think this is the cognitively easiest solution, despite its inefficiency, so I've gone with it.
		// I also don't know lisp... Or I'd add the endpoint to Memento.
		// If this ever happens, use this endpoint instead: https://github.com/codurance/memento/issues/62

		IEnumerable<MementoUser> users = (await _mementoClient.GetActiveInternalUsers())
			.Where(u => !_options.Value.WhiteList.Contains(u.Email));

		foreach (MementoUser user in users)
		{
			MementoTimeSheet timeSheet = await _mementoClient.GetTimeSheetForUser(user.Email, dateRange);

			if (timeSheet.IsComplete())
			{
				_logger.LogDebug("Timesheet for {UserEmail} is {Complete}", user.Email, "Complete");
				continue;
			}
			
			_logger.LogDebug("Timesheet for {UserEmail} is {Complete}", user.Email, "Incomplete");

			Language language = user.GetLanguage();
			TranslatedResourceCompoundKey key = new(tomorrowIsLastDay ? TranslatedResource.IndividualReminderMonthEndTemplate : TranslatedResource.IndividualReminderTemplate, language);
			
			string template = _translatedResourceService.GetTranslatedString(key);
			string message = template.Replace("{Person}", user.Name.Split(' ')[0]);
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
		
		_logger.LogDebug("Relevant dates for timesheet are {StartDate} to {EndDate}", startDate, endDate);

		return new(startDate, endDate);
	}
}