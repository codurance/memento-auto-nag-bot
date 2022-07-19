using System.Diagnostics.CodeAnalysis;
using MementoNagBot.Models.Misc;
using MementoNagBot.Services.Translation;
using Microsoft.Extensions.Logging.Abstractions;

namespace MementoNagBot.Tests.Services;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class StaticTranslatedResourceServiceTests
{
	public class GivenTheTranslatedResourceExists
	{
		[Theory]
		[InlineData(TranslatedResource.GeneralReminderTemplate, Language.English)]
		[InlineData(TranslatedResource.GeneralReminderTemplate, Language.French)]
		[InlineData(TranslatedResource.GeneralReminderTemplate, Language.Portuguese)]
		[InlineData(TranslatedResource.GeneralReminderTemplate, Language.Spanish)]
		
		[InlineData(TranslatedResource.GeneralReminderMonthEndTemplate, Language.English)]
		[InlineData(TranslatedResource.GeneralReminderMonthEndTemplate, Language.French)]
		[InlineData(TranslatedResource.GeneralReminderMonthEndTemplate, Language.Portuguese)]
		[InlineData(TranslatedResource.GeneralReminderMonthEndTemplate, Language.Spanish)]
		
		[InlineData(TranslatedResource.IndividualReminderTemplate, Language.English)]
		[InlineData(TranslatedResource.IndividualReminderTemplate, Language.French)]
		[InlineData(TranslatedResource.IndividualReminderTemplate, Language.Portuguese)]
		[InlineData(TranslatedResource.IndividualReminderTemplate, Language.Spanish)]
		
		[InlineData(TranslatedResource.IndividualReminderMonthEndTemplate, Language.English)]
		[InlineData(TranslatedResource.IndividualReminderMonthEndTemplate, Language.French)]
		[InlineData(TranslatedResource.IndividualReminderMonthEndTemplate, Language.Portuguese)]
		[InlineData(TranslatedResource.IndividualReminderMonthEndTemplate, Language.Spanish)]
		public void CanGetTranslatedResource(TranslatedResource resource, Language language)
		{
			ITranslatedResourceService service = new StaticTranslatedResourceService(NullLogger<StaticTranslatedResourceService>.Instance);

			string res = service.GetTranslatedString(new(resource, language));
			
			res.ShouldNotBeNullOrWhiteSpace();
		}
	}
}