using MementoNagBot.Clients.Memento;
using MementoNagBot.Clients.Slack;
using MementoNagBot.Extensions.DependencyInjection;
using MementoNagBot.Models.Options;
using MementoNagBot.Providers.DateTimes;
using MementoNagBot.Services.Gating;
using MementoNagBot.Services.Messaging;
using MementoNagBot.Services.Reminders;
using MementoNagBot.Services.Translation;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;


var builder = FunctionsApplication.CreateBuilder(args);
var config = builder.Configuration;
builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights()
    .Configure<BotOptions>(config.GetSection("Values:BotOptions"))
    .Configure<SlackOptions>(config.GetSection("Values:SlackOptions"))
    .Configure<MementoOptions>(config.GetSection("Values:MementoOptions"))
    .AddTransient<SlackMessageService>()
    .AddTransient<StartGateService>()
    .AddTransient<MementoReminderService>()
    .AddTransient<ISlackClient, SlackClientWrapper>()
    .AddTransient<IDateProvider, SystemDateProvider>()
    .AddTransient<ITranslatedResourceService, StaticTranslatedResourceService>()
    .RegisterPollyPolicies()
    .AddLogging(l =>
		{
			LoggerConfiguration lc = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.WriteTo.Console();
			l.AddSerilog(lc.CreateLogger());
		})
    .AddHttpClient<IMementoClient, MementoClient>(c =>
        {
            MementoOptions? options = config.GetSection("Values:MementoOptions").Get<MementoOptions>();
            if (options != null)
            {
                string baseUrl = options.MementoApiUrl.TrimEnd('/');
                if (!baseUrl.EndsWith("/api"))
                    baseUrl += "/api";
                c.BaseAddress = new(baseUrl + "/");
                c.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", options.MementoApiToken);
            }
        });

builder.Build().Run();
