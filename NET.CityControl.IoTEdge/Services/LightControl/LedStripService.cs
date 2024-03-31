using Iot.Device.FtCommon;
using NET.CityControl.IoTEdge.Models.LightControl;
using NET.CityControl.IoTEdge.Models.LightControl.Messages;
using NET.CityControl.IoTEdge.Providers;
using System.Drawing;

namespace NET.CityControl.IoTEdge.Services.LightControl;

internal class LedStripService
{
    private Task? _knightRiderTask;
    private CancellationTokenSource? _knightRiderCts;
    private Task? _rainbowTask;
    private CancellationTokenSource? _rainbowCts;

    private int _numberOfLeds;

    private IProvider? _spiProvider;
    private IProviderFactory _factory;

    public LedStripService(IProviderFactory factory, int numberOfLeds)
    {
        _factory = factory;
        _numberOfLeds = numberOfLeds;
    }

    public async void HandleLedStripMessage<T>(T message, CancellationToken cancellationToken) where T : ILedStripBaseMessage
    {
        if (message == null)
        {
            Console.WriteLine("Empty message received");
            return;
        }

        _spiProvider = await _factory.GetSerialProviderAsync("SPI", cancellationToken);
        var ledDevice = _spiProvider.GetLedDevice();

        if (ledDevice == null)
        {
            throw new ArgumentException($"Please instantiate {nameof(LedStripService)} withcorrect SPI Connection Settings");
        }

        switch (message)
        {
            case LedStripResetMessage ledStripResetMessage:
                _rainbowCts?.Cancel();
                _knightRiderCts?.Cancel();

                var img = ledDevice.Image;
                img.Clear();
                ledDevice.Update();
                break;
            case LedStripSetLightningMessage lightningMessage:
                SetPixels(Color.FromArgb(0, lightningMessage.R, lightningMessage.G, lightningMessage.B), lightningMessage.StartIndex, lightningMessage.Length);
                break;
            case LedStripActionMessage ledStripActionMessage:
                if (ledStripActionMessage.LedStripAction == LedStripActions.KnightRider)
                {
                    _rainbowCts?.Cancel();

                    _knightRiderCts = new CancellationTokenSource();
                    _knightRiderTask = new Task(async () => await LedStripActionHandlers.KnightRiderAsync(ledDevice, _numberOfLeds, _knightRiderCts.Token));
                    _knightRiderTask.Start();
                }

                if (ledStripActionMessage.LedStripAction == LedStripActions.Rainbow)
                {
                    _knightRiderCts?.Cancel();

                    _rainbowCts = new CancellationTokenSource();
                    _rainbowTask = new Task(async () => await LedStripActionHandlers.RainbowAsync(ledDevice, _numberOfLeds, _rainbowCts.Token));
                    _rainbowTask.Start();
                }
                break;
            default:
                throw new NotImplementedException("Message Type not implemented");
        }
    }

    private void SetPixels(Color color, int startLEDIndex, int length)
    {
        var ledDevice = _spiProvider?.GetLedDevice();

        if (length > _numberOfLeds)
        {
            length = _numberOfLeds;
        }

        Console.WriteLine($"SetPixels: Start LED Index: {startLEDIndex} - Length: {length} - Color: {color}");

        var bitmapImage = ledDevice?.Image;

        for (int i = startLEDIndex; i < startLEDIndex + length; i++)
        {
            bitmapImage?.SetPixel(i, 0, color);
        }

        ledDevice?.Update();
    }

    private static void PrintDeviceInfo(FtDevice device)
    {
        Console.WriteLine($"{device.Description}");
        Console.WriteLine($" Flags: {device.Flags}");
        Console.WriteLine($" Id: {device.Id}");
        Console.WriteLine($" LocId: {device.LocId}");
        Console.WriteLine($" Serial number: {device.SerialNumber}");
        Console.WriteLine($" Type: {device.Type}");
    }
}

