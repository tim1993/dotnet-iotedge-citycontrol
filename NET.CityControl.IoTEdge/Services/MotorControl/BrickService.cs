using Iot.Device.BuildHat.Models;
using NET.CityControl.IoTEdge.Models.MotorControl;
using NET.CityControl.IoTEdge.Models.MotorControl.Messages;
using NET.CityControl.IoTEdge.Providers;
using System.Globalization;

namespace NET.CityControl.IoTEdge.Services.MotorControl;

internal class BrickService: IDisposable
{
    private IProvider? _serialProvider;
    private bool _disposed;
    private double? _currentMotorSpeed;
    private readonly SensorPort _windMotor;
    private const int MotorThreshold = 10;

    private IProviderFactory _factory;

    public BrickService(IProviderFactory factory, SensorPort windMotor)
    {
        _factory = factory;
        _windMotor = windMotor;
    }

    public string Port => _serialProvider?.GetSerialPort() ?? "";

    public async Task HandleMessageAsync(BuildHatMessage message, CancellationToken cancellationToken)
    {
        _serialProvider = await _factory.GetSerialProviderAsync("SERIAL", cancellationToken);
        if (message is null)
        {
            return;
        }

        if (string.IsNullOrEmpty(_serialProvider?.GetSerialPort()))
        {
            throw new Exception("Please instantiate a new instance of BrickService with the correct serial port.");
        }

        SwitchMotorSpeed(message.MotorSpeed);
        _serialProvider?.Dispose();
        _factory.ReleaseLock();
    }

    public async Task HandleMotorControlMessageAsync(IMotorControlMessage message, CancellationToken cancellationToken)
    {
        _serialProvider = await _factory.GetSerialProviderAsync("SERIAL", cancellationToken);

        var relativeMotorSpeed = message.MotorSpeed;

        _currentMotorSpeed = relativeMotorSpeed > MotorThreshold ? relativeMotorSpeed : _currentMotorSpeed;
        if (_currentMotorSpeed > MotorThreshold)
        {
            SwitchMotorSpeed(Convert.ToInt32(_currentMotorSpeed ?? 0));
        }

        _serialProvider?.Dispose();
        _factory.ReleaseLock();
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void SwitchMotorSpeed(int speed)
    {
        _serialProvider?.GetBrick()?.SendRawCommand($"port {(byte)_windMotor} ; plimit 1\r");
        _serialProvider?.GetBrick()?.SendRawCommand($"port {(byte)_windMotor} ; bias 0.1\r");

        var speedCommand = $"port {(byte)_windMotor} ; pwm ; set {(speed / 100.0).ToString(CultureInfo.InvariantCulture)}\r";
        _serialProvider?.GetBrick()?.SendRawCommand(speedCommand);
        Console.WriteLine(speedCommand);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _serialProvider?.Dispose();
            }

            _disposed = true;
        }
    }
}
