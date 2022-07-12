using System;
using MementoNagBot.Services;
using MementoNagBot.Wrappers;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(MementoNagBot.Startup))]
namespace MementoNagBot;

public class Startup: FunctionsStartup
{
	public override void Configure(IFunctionsHostBuilder builder)
	{
		builder.Services.AddTransient<SlackMessageService>();
		builder.Services.AddTransient<ISlackClient, SlackClientWrapper>(_ => new(new(Environment.GetEnvironmentVariable("SLACK_API_TOKEN"))));
	}
}