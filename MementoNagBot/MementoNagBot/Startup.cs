using System.IO;
using MementoNagBot.Clients.Slack;
using MementoNagBot.Models.Options;
using MementoNagBot.Providers.DateTimes;
using MementoNagBot.Services.Gating;
using MementoNagBot.Services.Messaging;
using MementoNagBot.Services.Reminders;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

[assembly: FunctionsStartup(typeof(MementoNagBot.Startup))]
namespace MementoNagBot;

public class Startup: FunctionsStartup
{
	public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
	{
		string localSettingsPath = Path.Combine(builder.GetContext().ApplicationRootPath, "local.settings.json"); // This feels like it shouldn't be needed...
		builder.ConfigurationBuilder.AddJsonFile(localSettingsPath, true, true);
		base.ConfigureAppConfiguration(builder);
	}

	public override void Configure(IFunctionsHostBuilder builder)
	{
		IConfiguration config = builder.GetContext().Configuration;
		
		builder.Services.AddLogging(l =>
		{
			LoggerConfiguration lc = new();
			l.AddSerilog(lc.CreateLogger());
		});

		builder.Services.Configure<BotOptions>(config.GetSection("Values:BotOptions"));
		builder.Services.Configure<SlackOptions>(config.GetSection("Values:SlackOptions"));
		builder.Services.Configure<MementoOptions>(config.GetSection("Values:MementoOptions"));
		
		builder.Services.AddTransient<SlackMessageService>();
		builder.Services.AddTransient<StartGateService>();
		builder.Services.AddTransient<MementoReminderService>();
		
		builder.Services.AddTransient<ISlackClient, SlackClientWrapper>();
		builder.Services.AddTransient<IDateProvider, SystemDateProvider>();
	}
}