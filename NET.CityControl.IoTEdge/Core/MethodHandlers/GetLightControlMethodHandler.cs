using Microsoft.Azure.Devices.Client;
using NET.CityControl.IoTEdge.Models.LightControl.Messages;
using NET.CityControl.IoTEdge.Services.LightControl;
using System.Net;
using System.Text;
using System.Text.Json;

namespace NET.CityControl.IoTEdge.Core.MethodHandlers
{
    internal class GetLightControlMethodHandler(LedStripService ledStripService) : MethodHandlerBase("GetLightControlMethodHandler")
    {
        private readonly LedStripService? _ledStripService = ledStripService;

        protected override async Task<MethodResponse> HandleMethodInternalAsync(MethodRequest methodRequest, CancellationToken cancellationToken)
        {
            byte[] messageBytes = methodRequest.Data;
            string messageString = Encoding.UTF8.GetString(messageBytes);
            Console.WriteLine($"IoT-Hub: Direct Method Call received - Body: [{messageString}]");

            if (!string.IsNullOrEmpty(messageString))
            {
                using (var pipeMessage = new Message(messageBytes))
                {
                    var lightMessage = JsonSerializer.Deserialize<ILedStripBaseMessage>(messageString);

                    if (lightMessage != null)
                    {
                        _ledStripService?.HandleLedStripMessage(lightMessage, cancellationToken);
                    }
                    else
                    {
                        Console.WriteLine($"IoT-Hub: Received message couldn't be parsed, Body: [{messageString}]");
                        return new MethodResponse(500);
                    }
                }
            }
            return new MethodResponse((int)HttpStatusCode.OK);
        }
    }
}
