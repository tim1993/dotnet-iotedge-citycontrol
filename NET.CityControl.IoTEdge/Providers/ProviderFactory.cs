using NET.CityControl.IoTEdge.Models.LightControl;
using NET.CityControl.IoTEdge.Models.MotorControl;
using NET.CityControl.IoTEdge.Services.Host.Terminal;

namespace NET.CityControl.IoTEdge.Providers
{
    internal class ProviderFactory : IProviderFactory
    {
        private readonly LedSettings _lightSettings;
        private readonly MotorSettings _motorSettings;
        private readonly ITerminalManager _terminalManager;

        private static SemaphoreSlim _semaphore = new(1, 1);

        public ProviderFactory(LedSettings lightSettings, MotorSettings motorSettings, ITerminalManager terminalManager)
        {
            _lightSettings = lightSettings;
            _motorSettings = motorSettings;
            _terminalManager = terminalManager;
        }

        public async Task<IProvider> GetSerialProviderAsync(string mode, CancellationToken cancellationToken)
        {
            switch(mode)
            {
                case "SPI":
                    await _semaphore.WaitAsync();
                    //await _terminalManager.DisableSerialModeAsync(cancellationToken);
                    return new SPIProvider(_lightSettings.LightUsbId, _lightSettings.LedStripLength);
                case "SERIAL":
                    await _semaphore.WaitAsync();
                    //await _terminalManager.EnableSerialModeAsync(_motorSettings.BuildHatPortMoxa, cancellationToken);
                    return new SerialProvider(_motorSettings.BuildHatPort);
                default:
                    throw new ArgumentException($"No Provider for {mode} available");
            }
        }

        public void ReleaseLock()
        {
            _semaphore.Release();
        }

        public void Dispose()
        {
            _semaphore.Release();
            _terminalManager?.Dispose();
        }
    }
}
