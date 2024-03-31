
using Microsoft.Extensions.Configuration;
using NET.CityControl.IoTEdge.Models;
using NET.CityControl.IoTEdge.Models.LightControl;
using NET.CityControl.IoTEdge.Models.MotorControl;

namespace NET.CityControl.IoTEdge.Services;

internal class SettingsService : ISettingsService
{
    static IConfiguration _configuration;
    static SettingsService()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();
    }

    public LedSettings? GetLedSettings()
    {
        var config = _configuration.GetRequiredSection("LedSettings").Get<LedSettings>();
        if(config == null)
        {
            Console.WriteLine("LedSettings not found in appsettings.json");
            return null;
        }

        return config;
    }

    public MotorSettings? GetMotorSettings()
    {
        var config = _configuration.GetRequiredSection("MotorSettings").Get<MotorSettings>();
        if (config == null)
        {
            Console.WriteLine("MotorSettings not found in appsettings.json");
            return null;
        }

        return config;
    }

    public DeviceSettings? GetDeviceSettings()
    {
        var config = _configuration.GetRequiredSection("DeviceSettings").Get<DeviceSettings>();
        if (config == null)
        {
            Console.WriteLine("DeviceSettings not found in appsettings.json");
            return null;
        }

        return config;
    }
}
