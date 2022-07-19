using MementoNagBot.Models.Misc;
using MementoNagBot.Services.Translation;

namespace MementoNagBot.Tests.Stubs;

public class TranslationServiceStub : ITranslatedResourceService
{
	public static string MonthEndReminderText = "MonthEndReminder";
	public static string FridayReminderText = "FridayReminderText";
	public static string IndividualFridayReminderText = "IndividualFridayReminderText";
	public static string IndividualMonthEndReminderText = "IndividualMonthEndReminderText";

	private readonly Dictionary<TranslatedResource, string> _resources = new()
	{
		{ TranslatedResource.IndividualReminderTemplate, IndividualFridayReminderText },
		{ TranslatedResource.GeneralReminderTemplate, FridayReminderText },
		{ TranslatedResource.IndividualReminderMonthEndTemplate, IndividualMonthEndReminderText },
		{ TranslatedResource.GeneralReminderMonthEndTemplate, MonthEndReminderText }
	};

	public string GetTranslatedString(TranslatedResourceCompoundKey compoundKey) => $"{_resources[compoundKey.ResourceKey]}{compoundKey.Language}";
}