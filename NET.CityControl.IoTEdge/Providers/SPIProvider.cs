using Iot.Device.BuildHat;
using Iot.Device.Ft232H;
using Iot.Device.FtCommon;
using Iot.Device.Ws28xx;
using System.Device.Gpio;
using System.Device.Spi;

namespace NET.CityControl.IoTEdge.Providers
{
    internal class SPIProvider : IProvider, IDisposable
    {
        private bool _isDisposing;
        private readonly SpiDevice? _spiDevice;
        private readonly Ft232HDevice? _ft232hDevice;
        private readonly SpiConnectionSettings _settings;
        private Ws2812b? _ledDevice;

        public SPIProvider(string lightUsbId, int numberOfLeds)
        {
            var devices = FtCommon.GetDevices();
            var ftDevice = devices.SingleOrDefault(x => x.Id == Convert.ToUInt32(lightUsbId, 10));
            _settings = new(0, 3) { ClockFrequency = 2_400_000, ChipSelectLineActiveState = PinValue.Low, Mode = SpiMode.Mode0, BusId = 2 }; 
            
            if (ftDevice == null)
            {
                throw new Exception("Error: Initialization not possible, FT232H Converter Device must be plugged in via USB.");
            }

            PrintDeviceInfo(ftDevice);

            _ft232hDevice = new Ft232HDevice(ftDevice);
            _spiDevice = _ft232hDevice.CreateSpiDevice(_settings);
            _ledDevice = new Ws2812b(_spiDevice, numberOfLeds);
        }

        public void Dispose() => Dispose(true);

        private static void PrintDeviceInfo(FtDevice device)
        {
            Console.WriteLine($"{device.Description}");
            Console.WriteLine($" Flags: {device.Flags}");
            Console.WriteLine($" Id: {device.Id}");
            Console.WriteLine($" LocId: {device.LocId}");
            Console.WriteLine($" Serial number: {device.SerialNumber}");
            Console.WriteLine($" Type: {device.Type}");
        }

        private void Dispose(bool disposing)
        {
            if (!_isDisposing)
            {
                if (disposing)
                {
                    _ft232hDevice?.Dispose();
                    _spiDevice?.Dispose();
                    _ledDevice = null;
                }

                _isDisposing = true;
            }
        }

        public Ws2812b GetLedDevice()
        {
            return _ledDevice!;
        }

        public Brick? GetBrick()
        {
            throw new NotImplementedException();
        }

        public string GetSerialPort()
        {
            throw new NotImplementedException();
        }
    }
}
