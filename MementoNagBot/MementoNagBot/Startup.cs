using System.IO;
using MementoNagBot.Options;
using MementoNagBot.Services;
using MementoNagBot.Wrappers;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
		
		builder.Services.Configure<BotOptions>(config.GetSection("Values:BotOptions"));
		builder.Services.Configure<SlackOptions>(config.GetSection("Values:SlackOptions"));
		
		builder.Services.AddTransient<SlackMessageService>();
		builder.Services.AddTransient<ISlackClient, SlackClientWrapper>();
	}
}