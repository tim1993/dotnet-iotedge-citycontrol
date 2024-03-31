using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using Microsoft.Extensions.Hosting;
using NET.CityControl.IoTEdge.Core.MessageHandlers;
using NET.CityControl.IoTEdge.Providers;
using NET.CityControl.IoTEdge.Services;
using NET.CityControl.IoTEdge.Services.Host.Terminal;
using NET.CityControl.IoTEdge.Services.LightControl;
using NET.CityControl.IoTEdge.Services.MotorControl;
using System.Reflection;
using System.Runtime.Loader;

namespace NET.CityControl.IoTEdge
{
    internal class ConsoleHostedService : IHostedService
    {
        private ModuleClient? _ioTHubModuleClient;
        private DeviceClient? _iotHubDeviceClient;

        private ISettingsService _settingsService;

        private ITerminalManager? _terminalManager;
        private BrickService? _brickService;
        private LedStripService? _ledStripService;

        private Version? _version;
        private CancellationTokenSource _cts;

        public ConsoleHostedService(ISettingsService settingsService)
        {
            _cts = new CancellationTokenSource();
            _settingsService = settingsService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            InitCityControl();
            await InitIoTEdgeAsync(_cts.Token);

            // Wait until the app unloads or is cancelled
            AssemblyLoadContext.Default.Unloading += (ctx) => _cts.Cancel();
            Console.CancelKeyPress += (sender, cpe) => _cts.Cancel();
            await WhenCancelled(_cts.Token);
        }

        public async Task StopAsync(CancellationToken cancellationToken) => await _cts.CancelAsync();

        /// <summary>
        /// Initializes the ModuleClient and sets up the callback to receive
        /// messages containing temperature information
        /// </summary>
        private void InitCityControl()
        {
            // Get current version number
            _version = Assembly.GetExecutingAssembly().GetName().Version;

            if (_settingsService.GetLedSettings() == null)
            {
                Console.WriteLine("Error: Initialization not possible, led settings are not available.");
                Exit();
                return;
            }

            if (_settingsService.GetMotorSettings() == null)
            {
                Console.WriteLine("Error: Initialization not possible, motor settings are not available.");
                Exit();
                return;
            }

            if (_settingsService.GetDeviceSettings() == null)
            {
                Console.WriteLine("Error: Initialization not possible, device settings are not available.");
                Exit();
                return;
            }

            Console.WriteLine("Initialization of City Control was successful");
            Console.WriteLine($"Version {_version}");
        }

        private async Task InitIoTEdgeAsync(CancellationToken cancellationToken)
        {
            MqttTransportSettings mqttSetting = new(TransportType.Mqtt_WebSocket_Only);
            ITransportSettings[] settings = [mqttSetting];

            try
            {
                _iotHubDeviceClient = DeviceClient.CreateFromConnectionString(_settingsService.GetDeviceSettings()?.DeviceConnectionString);
                await _iotHubDeviceClient.OpenAsync(cancellationToken);
                Console.WriteLine("Device client successfully initialized.");

                // Open a connection to the Edge runtime
                _ioTHubModuleClient = await ModuleClient.CreateFromEnvironmentAsync(settings);
                await _ioTHubModuleClient.OpenAsync(cancellationToken);
                Console.WriteLine("IoT Hub module client successfully initialized.");

                var httpClient = new HttpClient();

                // Register callbacks to be called when a message / methodcall by the module or C2D messages received
                var getLightControlMessageHandler = new CityControlMessageHandler(httpClient, _settingsService.GetLedSettings()?.LightWebServiceUrl, _settingsService.GetMotorSettings()?.MotorWebServiceUrl);
                await _iotHubDeviceClient.SetReceiveMessageHandlerAsync((message, userContext) => getLightControlMessageHandler.HandleMessageAsync(message, userContext, cancellationToken), _iotHubDeviceClient, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while initializing IoT Edge");
                Console.WriteLine(ex.ToString());
                Exit();
            }
        }

        private void SetupDirectControl()
        {
            // Initialize connection to Host for command execution
            var deviceSettings = _settingsService.GetDeviceSettings();
            var ledSettings = _settingsService.GetLedSettings();
            var motorSettings = _settingsService.GetMotorSettings();

            if (deviceSettings == null || ledSettings == null || motorSettings == null)
            {
                throw new InvalidOperationException("Device settings, LED Settings and Motor Settings must be set to setup direct control mode");
            }

            _terminalManager = new LinuxTerminalManager(new TerminalClient(deviceSettings.Host, deviceSettings.Port, deviceSettings.User, deviceSettings.Password));
            Console.WriteLine("Linux Terminal Session is initialized.");

            var factory = new ProviderFactory(ledSettings, motorSettings, _terminalManager);

            _ledStripService = new(factory, ledSettings.LedStripLength);
            _brickService = new(factory, motorSettings.MotorPort);

            if (_ledStripService == null)
            {
                Console.WriteLine("Error: Initialization not possible, LED Strip service could not be initialized with given FT232H Converter device.");
                Exit();
                return;
            }

            if (_brickService == null)
            {
                Console.WriteLine($"Error: Initialization not possible, Motor Control service could not be initialized on connected serial port {motorSettings.BuildHatPort}.");
                Exit();
                return;
            }
        }

        /// <summary>
        /// Handles cleanup operations when app is cancelled or unloads
        /// </summary>
        private static Task<bool> WhenCancelled(CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            _ = cancellationToken.Register(callback: s => ((TaskCompletionSource<bool>)s!).SetResult(true), tcs);
            return tcs.Task;
        }

        private static void Exit()
        {
            Console.WriteLine("Error occured restarting module");
            Environment.Exit(0);
        }
    }
}
