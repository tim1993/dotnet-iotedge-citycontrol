using Microsoft.Azure.Devices.Client;
using NET.CityControl.IoTEdge.Models.LightControl.Messages;
using NET.CityControl.IoTEdge.Models.MotorControl;
using NET.CityControl.IoTEdge.Models.MotorControl.Messages;
using System.Text;
using System.Text.Json;

namespace NET.CityControl.IoTEdge.Core.MessageHandlers
{
    internal class CityControlMessageHandler : MessageHandlerBase
    {
        private readonly HttpClient _httpClient;
        private const int MotorThreshold = 10;
        private double _currentMotorSpeed = 0;

        private readonly string _lightControlUrl;
        private readonly string _motorControlUrl;
        public CityControlMessageHandler(HttpClient httpClient, string? lightControlUrl, string? motorControlUrl) : base("CityControlMessageHandler")
        {
            if (lightControlUrl == null)
            {
                throw new ArgumentNullException(nameof(lightControlUrl), "LightControl URL was not set");
            }

            if (motorControlUrl == null)
            {
                throw new ArgumentNullException(nameof(motorControlUrl), "motorControl URL was not set");
            }

            _httpClient = httpClient;
            _lightControlUrl = lightControlUrl;
            _motorControlUrl = motorControlUrl;

        }

        protected override async Task<MessageResponse> HandleMessageInternalAsync(Message message, object userContext, CancellationToken cancellationToken)
        {
            if (userContext is not DeviceClient deviceClient)
            {
                throw new InvalidOperationException("UserContext doesn't contain expected values for DeviceClient");
            }

            string messageData = Encoding.UTF8.GetString(message.GetBytes());
            Console.WriteLine($"Received message - dCount {message.DeliveryCount} - msgId/corId/lockT {message.MessageId}/{message.CorrelationId}/{message.LockToken} - data {messageData}");

            message.Properties.TryGetValue("messagetype", out string? messageType);

            ILedStripBaseMessage? parsedLEDMessage = null;
            IMotorControlMessage? parsedMotorControlMessage = null;

            switch (messageType)
            {
                case LEDStripMessageType.LedStripActionMessage:
                    parsedLEDMessage = JsonSerializer.Deserialize<LedStripActionMessage>(messageData);
                    await RouteLightControlMessageAsync($"{_lightControlUrl}/action", parsedLEDMessage, cancellationToken);
                    break;
                case LEDStripMessageType.LedStripSetLightningMessage:
                    parsedLEDMessage = JsonSerializer.Deserialize<LedStripSetLightningMessage>(messageData);
                    await RouteLightControlMessageAsync($"{_lightControlUrl}/setlighting", parsedLEDMessage, cancellationToken);
                    break;
                case LEDStripMessageType.SetLedStripLengthMessage:
                    parsedLEDMessage = JsonSerializer.Deserialize<SetLedStripLengthMessage>(messageData);
                    await RouteLightControlMessageAsync($"{_lightControlUrl}/setLength", parsedLEDMessage, cancellationToken);
                    break;
                case LEDStripMessageType.LedStripResetMessage:
                    parsedLEDMessage = JsonSerializer.Deserialize<LedStripResetMessage>(messageData);
                    await RouteLightControlMessageAsync($"{_lightControlUrl}/reset", parsedLEDMessage, cancellationToken);
                    break;

                case MotorControlMessageType.SetMotorSpeedControlMessage:
                    parsedMotorControlMessage = JsonSerializer.Deserialize<SetMotorSpeedControlMessage>(messageData);
                    await RouteMotorControlMessageAsync($"{_motorControlUrl}/speed", parsedMotorControlMessage, cancellationToken);
                    break;
                case MotorControlMessageType.SetMotorSpeedForSecondsControlMessage:
                    parsedMotorControlMessage = JsonSerializer.Deserialize<SetMotorSpeedForSecondsControlMessage>(messageData);
                    await RouteMotorControlMessageAsync($"{_motorControlUrl}/runseconds", parsedMotorControlMessage, cancellationToken);
                    break;
                default:
                    throw new NotImplementedException($"The given message type {messageType} was not implemented yet");
            };

            await deviceClient.CompleteAsync(message, cancellationToken);

            return MessageResponse.Completed;
        }

        private async Task RouteMotorControlMessageAsync(string actionUrl, IMotorControlMessage? parsedMessage, CancellationToken cancellationToken)
        {
            if (parsedMessage != null)
            {
                await RouteMotorControlMessageToActionAsync(actionUrl, parsedMessage, cancellationToken);
            }
            else
            {
                Console.WriteLine("Could not parse IoTTwinMessage or empty message was sent.");
            }
        }

        private async Task RouteMotorControlMessageToActionAsync(string actionUrl, IMotorControlMessage message, CancellationToken cancellationToken)
        {
            if (message == null)
            {
                Console.WriteLine("Received message is null, no action available");
                return;
            }

            var relativeMotorSpeed = message?.MotorSpeed;

            _currentMotorSpeed = (relativeMotorSpeed > MotorThreshold || relativeMotorSpeed == 0) ? relativeMotorSpeed ?? 0 : 0;
            if (_currentMotorSpeed > MotorThreshold || _currentMotorSpeed == 0)
            {
                var buildHatMessage = new BuildHatMessage() { MotorSpeed = (int)_currentMotorSpeed };
                await _httpClient.PostAsync(actionUrl, new StringContent(JsonSerializer.Serialize(buildHatMessage), Encoding.UTF8, "application/json"), cancellationToken);
            }
        }
    

        private async Task RouteLightControlMessageAsync(string actionUrl, ILedStripBaseMessage? parsedMessage, CancellationToken cancellationToken)
        {
            if (parsedMessage != null)
            {
                await RouteLightControlMessageToActionAsync(actionUrl, parsedMessage, cancellationToken);
            }
            else
            {
                Console.WriteLine("Could not parse IoTTwinMessage or empty message was sent.");
            }
        }

        private async Task RouteLightControlMessageToActionAsync(string actionUrl, ILedStripBaseMessage message, CancellationToken cancellationToken)
        {
            if (message == null)
            {
                Console.WriteLine("Received message is null, no action available");
                return;
            }

            await _httpClient.PostAsync(actionUrl, new StringContent(JsonSerializer.Serialize(message), Encoding.UTF8, "application/json"), cancellationToken);
        }
    }
}
