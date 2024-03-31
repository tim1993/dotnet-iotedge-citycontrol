using Microsoft.Azure.Devices.Client;
using System.Net;
using System.Text;

namespace NET.CityControl.IoTEdge.Core.MethodHandlers
{
    internal abstract class MethodHandlerBase
    {
        private readonly string _methodName;

        protected MethodHandlerBase(string methodName)
        {
            _methodName = methodName;
        }

        public async Task<MethodResponse> HandleMethodAsync(MethodRequest methodRequest, CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine($"Started to handle method {_methodName}");
                var methodResponse = await HandleMethodInternalAsync(methodRequest, cancellationToken);
                Console.WriteLine($"Method {_methodName} was handled successfully.");

                return methodResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine($"Failed to handle method {_methodName}");

                return new(Encoding.UTF8.GetBytes($"Failed to handle method: {ex.Message}"), (int)HttpStatusCode.InternalServerError);
            }
        }

        protected abstract Task<MethodResponse> HandleMethodInternalAsync(MethodRequest methodRequest, CancellationToken cancellationToken);
    }
}
