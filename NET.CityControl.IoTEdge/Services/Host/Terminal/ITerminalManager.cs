namespace NET.CityControl.IoTEdge.Services.Host.Terminal
{
    internal interface ITerminalManager: IDisposable
    {
        bool IsConnected { get; }

        Task EnableSerialModeAsync(string port, CancellationToken cancellationToken);

        Task DisableSerialModeAsync(CancellationToken cancellationToken);
    }
}
