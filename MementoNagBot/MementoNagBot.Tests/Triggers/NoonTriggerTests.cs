using System.Diagnostics.CodeAnalysis;
using MementoNagBot.Services.Gating;
using MementoNagBot.Tests.Stubs;
using MementoNagBot.Triggers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging.Abstractions;

namespace MementoNagBot.Tests.Triggers;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class NoonTriggerTests
{
	public class GivenItIsNoon
	{
		public class AndTomorrowIsMonthEnd
		{
			[Fact]
			public async Task ThenSendGeneralReminderForMonthEnd()
			{
				DateProviderStub dateProviderStub = new(new(2022, 08, 30));
				StartGateService startGate = new(dateProviderStub, NullLogger<StartGateService>.Instance);
				MementoReminderServiceStub reminderService = new();
				TimerInfo ti = new();
				
				NoonTrigger trigger = new(startGate, reminderService, NullLogger<NoonTrigger>.Instance);
				await trigger.RunAsync(ti);

				reminderService.SentGeneralReminderMonthEnd.ShouldBeTrue();
				reminderService.SentGeneralReminderFriday.ShouldBeFalse();
				reminderService.SentIndividualReminderFriday.ShouldBeFalse();
				reminderService.SentIndividualReminderMonthEnd.ShouldBeFalse();
			}
		}

		public class AndTodayIsFridayAndTodayIsNotMonthEnd
		{
			[Fact]
			public async Task ThenSendGeneralReminderForFriday()
			{
				DateProviderStub dateProviderStub = new(new(2022, 07, 22));
				StartGateService startGate = new(dateProviderStub, NullLogger<StartGateService>.Instance);
				MementoReminderServiceStub reminderService = new();
				TimerInfo ti = new();
				
				NoonTrigger trigger = new(startGate, reminderService, NullLogger<NoonTrigger>.Instance);
				await trigger.RunAsync(ti);

				reminderService.SentGeneralReminderMonthEnd.ShouldBeFalse();
				reminderService.SentGeneralReminderFriday.ShouldBeTrue();
				reminderService.SentIndividualReminderFriday.ShouldBeFalse();
				reminderService.SentIndividualReminderMonthEnd.ShouldBeFalse();
			}
		}

		public class AndTodayIsNotFridayNorIsTomorrowMonthEnd
		{
			[Fact]
			public async Task ThenDoNothing()
			{
				DateProviderStub dateProviderStub = new(new(2022, 07, 20));
				StartGateService startGate = new(dateProviderStub, NullLogger<StartGateService>.Instance);
				MementoReminderServiceStub reminderService = new();
				TimerInfo ti = new();
				
				NoonTrigger trigger = new(startGate, reminderService, NullLogger<NoonTrigger>.Instance);
				await trigger.RunAsync(ti);

				reminderService.SentGeneralReminderMonthEnd.ShouldBeFalse();
				reminderService.SentGeneralReminderFriday.ShouldBeFalse();
				reminderService.SentIndividualReminderFriday.ShouldBeFalse();
				reminderService.SentIndividualReminderMonthEnd.ShouldBeFalse();
			}
		}
		
		public class AndTodayIsFridayAndMonthEnd
		{
			[Fact]
			public async Task ThenDoNothing()
			{
				DateProviderStub dateProviderStub = new(new(2022, 07, 29));
				StartGateService startGate = new(dateProviderStub, NullLogger<StartGateService>.Instance);
				MementoReminderServiceStub reminderService = new();
				TimerInfo ti = new();
				
				NoonTrigger trigger = new(startGate, reminderService, NullLogger<NoonTrigger>.Instance);
				await trigger.RunAsync(ti);

				reminderService.SentGeneralReminderMonthEnd.ShouldBeFalse();
				reminderService.SentGeneralReminderFriday.ShouldBeFalse();
				reminderService.SentIndividualReminderFriday.ShouldBeFalse();
				reminderService.SentIndividualReminderMonthEnd.ShouldBeFalse();
			}
		}
	}
}