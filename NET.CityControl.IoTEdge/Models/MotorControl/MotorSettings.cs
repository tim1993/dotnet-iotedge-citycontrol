using Iot.Device.BuildHat.Models;

namespace NET.CityControl.IoTEdge.Models.MotorControl;

internal class MotorSettings
{
    public string BuildHatPort { get; set; } = string.Empty;
    public SensorPort MotorPort { get; set; } = SensorPort.PortA;
    public string MotorWebServiceUrl { get; set; } = string.Empty;
}
