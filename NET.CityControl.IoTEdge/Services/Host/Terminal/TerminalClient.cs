using Renci.SshNet;

namespace NET.CityControl.IoTEdge.Services.Host.Terminal
{
    internal class TerminalClient : ITerminalClient
    {
        private readonly SshClient _sshClient;
        private bool _disposed;

        public TerminalClient(string host, int port, string username, string password)
        {
            _sshClient = new SshClient(host, port, username, password);
        }

        public bool isConnected => _sshClient.IsConnected;

        public Task ConnectAsync(CancellationToken cancellationToken)
        {
            _sshClient?.Connect();
            Console.WriteLine($"SSH connection to host{_sshClient?.ConnectionInfo.Host} on port {_sshClient?.ConnectionInfo.Port} was opened.");
            return Task.CompletedTask;
        }

        public Task DisconnectAsync(CancellationToken cancellationToken)
        {
            _sshClient?.Disconnect();
            Console.WriteLine("SSH connection to host was closed.");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async Task ExecuteAsync(string command, CancellationToken cancellationToken)
        {
            using var sshCommand = _sshClient?.RunCommand(command);
            if (sshCommand != null)
            {
                await Task.Factory.FromAsync((callback, state) => sshCommand.BeginExecute(callback, state), (x) => sshCommand?.EndExecute(x), null);
            }
            else
            {
                Console.WriteLine($"Couldn't create SSH command from string {command}");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _sshClient.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
