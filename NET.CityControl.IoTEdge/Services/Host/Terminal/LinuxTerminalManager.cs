namespace NET.CityControl.IoTEdge.Services.Host.Terminal
{
    internal class LinuxTerminalManager : ITerminalManager
    {
        private bool _disposed = false;
        private readonly ITerminalClient _terminalClient;

        public LinuxTerminalManager(ITerminalClient terminalClient)
        {
            _terminalClient = terminalClient;
            Initialize = TryConnectClientAsync(default);
        }

        public bool IsConnected => _terminalClient.isConnected;

        public Task Initialize { get; }

        public async Task DisableSerialModeAsync(CancellationToken cancellationToken)
        {
            await TryConnectClientAsync(cancellationToken);
            await _terminalClient.ExecuteAsync("sudo rmmod ftdi_sio", cancellationToken);
        }

        public async Task EnableSerialModeAsync(string port, CancellationToken cancellationToken)
        {
            await TryConnectClientAsync(cancellationToken);
            await _terminalClient.ExecuteAsync("sudo /sbin/modprobe ftdi_sio", cancellationToken);
            await _terminalClient.ExecuteAsync($"sudo chmod 777 {port}", cancellationToken);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private async Task TryConnectClientAsync(CancellationToken cancellationToken)
        {
            if (!IsConnected)
            {
                await _terminalClient.ConnectAsync(cancellationToken);
            }
        }

        protected async virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    await _terminalClient.DisconnectAsync(default);
                    _terminalClient.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
