using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET.CityControl.IoTEdge.Services.Host.Terminal
{
    internal interface ITerminalClient: IDisposable
    {
        bool isConnected { get; }

        Task ConnectAsync(CancellationToken cancellationToken);

        Task DisconnectAsync(CancellationToken cancellationToken);

        Task ExecuteAsync(string command, CancellationToken cancellationToken);
    }
}
