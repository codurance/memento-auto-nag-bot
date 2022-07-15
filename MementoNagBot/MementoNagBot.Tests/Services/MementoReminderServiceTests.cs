using System.Diagnostics.CodeAnalysis;
using MementoNagBot.Services.Reminders;
using MementoNagBot.Tests.Stubs;


namespace MementoNagBot.Tests.Services;


[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class MementoReminderServiceTests
{
	public class GivenTomorrowIsMonthEnd
	{
		public class WhenISendAGeneralReminder
		{
			[Fact]
			public async Task ThenTheMonthEndMessageIsSent()
			{
				SlackMessageServiceStub stub = new(null!, null!);
				MementoReminderService service = new(stub);

				await service.SendGeneralReminder(true);

				stub.LastBotChannelMessage.ShouldBe(MementoReminderService.MonthEndReminderText);
			}
		}

		public class WhenISendAnIndividualReminder
		{
			
		}
	}

	public class GivenTomorrowIsNotMonthEnd
	{
		public class WhenISendAGeneralReminder
		{
			[Fact]
			public async Task ThenTheGeneralReminderMessageIsSent()
			{
				SlackMessageServiceStub stub = new(null!, null!);
				MementoReminderService service = new(stub);

				await service.SendGeneralReminder(false);

				stub.LastBotChannelMessage.ShouldBe(MementoReminderService.GeneralReminderText);
			}
			
		}

		public class WhenISendAnIndividualReminder
		{

		}
	}
}
