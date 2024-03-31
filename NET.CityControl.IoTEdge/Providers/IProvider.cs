using Iot.Device.BuildHat;
using Iot.Device.Ws28xx;

namespace NET.CityControl.IoTEdge.Providers
{
    internal interface IProvider: IDisposable
    {
        public Ws2812b GetLedDevice();

        public Brick? GetBrick();

        public string? GetSerialPort();
    }
}
