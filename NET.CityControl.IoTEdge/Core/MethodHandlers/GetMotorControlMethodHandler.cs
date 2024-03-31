using Microsoft.Azure.Devices.Client;
using NET.CityControl.IoTEdge.Models.MotorControl;
using NET.CityControl.IoTEdge.Services.MotorControl;
using System.Net;
using System.Text;
using System.Text.Json;

namespace NET.CityControl.IoTEdge.Core.MethodHandlers
{
    internal class GetMotorControlMethodHandler(BrickService brickService) : MethodHandlerBase("GetMotorControlMethodHandler")
    {
        private readonly BrickService _brickService = brickService;

        protected override async Task<MethodResponse> HandleMethodInternalAsync(MethodRequest methodRequest, CancellationToken cancellationToken)
        {
            byte[] messageBytes = methodRequest.Data;
            string messageString = Encoding.UTF8.GetString(messageBytes);
            Console.WriteLine($"IoT-Hub: Direct Method Call received - Body: [{messageString}]");

            if (!string.IsNullOrEmpty(messageString))
            {
                using (var pipeMessage = new Message(messageBytes))
                {
                    var buildHatMessage = JsonSerializer.Deserialize<BuildHatMessage>(messageString);

                    if (buildHatMessage != null)
                    {
                        await _brickService.HandleMessageAsync(buildHatMessage, cancellationToken);
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
