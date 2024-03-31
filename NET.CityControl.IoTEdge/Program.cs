using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NET.CityControl.IoTEdge;
using NET.CityControl.IoTEdge.Services;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<ConsoleHostedService>();
        services.AddSingleton<ISettingsService, SettingsService>();
    })
    .RunConsoleAsync();