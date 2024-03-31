namespace NET.CityControl.IoTEdge.Models
{
    internal class DeviceSettings
    {
        public string DeviceConnectionString { get; set; } = string.Empty;

        public string Host { get; set; } = string.Empty;

        public int Port { get; set; } = int.MinValue;

        public string User { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}