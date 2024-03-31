using Microsoft.Azure.Devices.Client;

namespace NET.CityControl.IoTEdge.Core.MessageHandlers
{
    internal abstract class MessageHandlerBase
    {
        private readonly string _messageName;

        protected MessageHandlerBase(string messageName)
        {
            _messageName = messageName;
        }

        public async Task<MessageResponse> HandleMessageAsync(Message message, object userContext, CancellationToken cancellationToken)
        {
            try
            {
                var methodResponse = await HandleMessageInternalAsync(message, userContext, cancellationToken);
                return methodResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine($"Failed to handle method {_messageName}");
            }

            return MessageResponse.Completed;
        }

        protected abstract Task<MessageResponse> HandleMessageInternalAsync(Message message, object userContext, CancellationToken cancellationToken);
    }
}
