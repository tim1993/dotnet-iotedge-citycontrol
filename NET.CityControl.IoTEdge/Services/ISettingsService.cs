using NET.CityControl.IoTEdge.Models;
using NET.CityControl.IoTEdge.Models.LightControl;
using NET.CityControl.IoTEdge.Models.MotorControl;

namespace NET.CityControl.IoTEdge.Services
{
    internal interface ISettingsService
    {
        public LedSettings? GetLedSettings();
        public MotorSettings? GetMotorSettings();
        public DeviceSettings? GetDeviceSettings();
    }
}
