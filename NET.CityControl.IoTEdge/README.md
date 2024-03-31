# Dotnet IoTEdge CityControl
This project enables you to deploy an IoT Edge Container that can control LED and Motor over IoT Hub Messages.

**! ! ! This project is currently under development. It's more like an solution accelerator than a perfect ready to go solution. ! ! !**

There are two operating modes are implemented. The first is that the edge gateway only forwards the messages to other devices via web requests and therefore only acts as a router. 
The second scenario is that the LED strip and the motor are connected and controlled directly to the Edge Gateway.

# Create Docker Container and push
docker build -t net-citycontrol-iotedge:1.0 .  
docker tag net-citycontrol-iotedge:1.0 <your.azurecr.io>/citycontrol/net-citycontrol-iotedge:1.0  
az acr login --name <your.azurecr.io>  
docker push <your.azurecr.io>/citycontrol/net-citycontrol-iotedge:1.0  

# Using with web request router
Everything is already set up for this scenario. Each message is forwarded to the CityControlMessageHandler and the corresponding web service is called depending on the message type. 
It is important to configure the corresponding interfaces in appsettings.json
```
{
  "DeviceSettings": {
    "DeviceConnectionString": "<Device-Connection-String>",
    "Host": "127.0.0.1",
    "Port": "22",
    "User": "<your-user>",
    "Password": "<your-super-secret-password>"
  },
  "LedSettings": {
    "LightUsbId": "0",
    "LedStripLength": 175,
    "LightWebServiceUrl": "http://192.168.2.208:7574/ledcontrol"
  },
  "MotorSettings": {
    "BuildHatPort": "/dev/ttyUSB0",
    "WindMotorPort": "PortA",
    "MotorWebServiceUrl": "http://192.168.2.208:7573/motorcontrol"
  }
}
```

# Using with direct control
You need to call the SetupDirectControl method in ConsoleHostedService to set up the direct control services.
The corresponding services that you need to control LED strips or a motor will then run. 
The variables _ledStripService and _brickService are the respective service instances. 
Further actions such as C2D, direct method calls, etc. still need to be implemented by you. 
This is best done in the InitIoTEdgeAsync method of ConsoleHostedService.

## Pitfalls in Linux:
There is a need to enable / disable kernel modules for SPI mode or serial mode:
For SPI ->  ```sudo rmmod ftdi_sio```
For Serial Mode -> ```/sbin/modprobe ftdi_sio```

The direct control mode is setting that for you automatically during your session.
For web request router mode you don't need to worry about if you are using separate devices.