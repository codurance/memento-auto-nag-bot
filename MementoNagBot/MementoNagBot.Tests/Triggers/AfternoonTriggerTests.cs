using System.Diagnostics.CodeAnalysis;
using MementoNagBot.Services.Gating;
using MementoNagBot.Tests.Stubs;
using MementoNagBot.Triggers;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Timers;
using Microsoft.Extensions.Logging.Abstractions;
using NCrontab;

namespace MementoNagBot.Tests.Triggers;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class AfternoonTriggerTests
{
	public class GivenItIsSixteenHundredHours
	{
		public class AndTomorrowIsMonthEnd
		{
			[Fact]
			public async Task ThenSendIndividualReminderForMonthEnd()
			{
				DateProviderStub dateProviderStub = new(new(2022, 08, 30));
				StartGateService startGate = new(dateProviderStub, NullLogger<StartGateService>.Instance);
				MementoReminderServiceStub reminderService = new();
				TimerInfo ti = new(new CronSchedule(CrontabSchedule.Parse(NoonTrigger.ScheduleExpression)), new(), true);
				
				AfternoonTrigger trigger = new(startGate, reminderService, NullLogger<AfternoonTrigger>.Instance);
				await trigger.RunAsync(ti, NullLogger.Instance);

				reminderService.SentGeneralReminderMonthEnd.ShouldBeFalse();
				reminderService.SentGeneralReminderFriday.ShouldBeFalse();
				reminderService.SentIndividualReminderFriday.ShouldBeFalse();
				reminderService.SentIndividualReminderMonthEnd.ShouldBeTrue();
			}
		}

		public class AndTodayIsFridayAndTodayIsNotMonthEnd
		{
			[Fact]
			public async Task ThenSendIndividualReminderForFriday()
			{
				DateProviderStub dateProviderStub = new(new(2022, 07, 22));
				StartGateService startGate = new(dateProviderStub, NullLogger<StartGateService>.Instance);
				MementoReminderServiceStub reminderService = new();
				TimerInfo ti = new(new CronSchedule(CrontabSchedule.Parse(NoonTrigger.ScheduleExpression)), new(), true);
				
				AfternoonTrigger trigger = new(startGate, reminderService, NullLogger<AfternoonTrigger>.Instance);
				await trigger.RunAsync(ti, NullLogger.Instance);

				reminderService.SentGeneralReminderMonthEnd.ShouldBeFalse();
				reminderService.SentGeneralReminderFriday.ShouldBeFalse();
				reminderService.SentIndividualReminderFriday.ShouldBeTrue();
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
				TimerInfo ti = new(new CronSchedule(CrontabSchedule.Parse(NoonTrigger.ScheduleExpression)), new(), true);
				
				NoonTrigger trigger = new(startGate, reminderService, NullLogger<NoonTrigger>.Instance);
				await trigger.RunAsync(ti, NullLogger.Instance);

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
				TimerInfo ti = new(new CronSchedule(CrontabSchedule.Parse(NoonTrigger.ScheduleExpression)), new(), true);
				
				NoonTrigger trigger = new(startGate, reminderService, NullLogger<NoonTrigger>.Instance);
				await trigger.RunAsync(ti, NullLogger.Instance);

				reminderService.SentGeneralReminderMonthEnd.ShouldBeFalse();
				reminderService.SentGeneralReminderFriday.ShouldBeFalse();
				reminderService.SentIndividualReminderFriday.ShouldBeFalse();
				reminderService.SentIndividualReminderMonthEnd.ShouldBeFalse();
			}
		}
	}
}