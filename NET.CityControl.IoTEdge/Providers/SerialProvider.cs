using Iot.Device.BuildHat;
using Iot.Device.Ws28xx;
using System.IO.Ports;

namespace NET.CityControl.IoTEdge.Providers
{
    internal class SerialProvider : IProvider, IDisposable
    {
        private Brick? _brick;
        private string? _port;
        private bool _isDisposing;

        public SerialProvider(string port)
        {
            try
            {
                string[] ports = SerialPort.GetPortNames();
                Console.WriteLine(string.Join(",", ports));
                Console.WriteLine($"Try to enable serial port connection with port: {port}.");
                _port = port;
                _brick = new(port);
                Console.WriteLine($"Serial Communication established with port: {port}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Dispose();
            }
        }

        public void Dispose() => Dispose(true);

        public Brick? GetBrick()
        {
            return _brick;
        }

        public Ws2812b GetLedDevice()
        {
            throw new NotImplementedException();
        }

        public string? GetSerialPort()
        {
            return _port;
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposing)
            {
                if (disposing)
                {
                    _brick?.Dispose();
                    Console.WriteLine("Brick disposed!");
                }

                _isDisposing = true;
            }
        }
    }
}
