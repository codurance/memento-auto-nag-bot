using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(MementoNagBot.Startup))]
namespace MementoNagBot;

public class Startup: FunctionsStartup
{
	public override void Configure(IFunctionsHostBuilder builder)
	{
		
	}
}