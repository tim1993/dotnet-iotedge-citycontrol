namespace NET.CityControl.IoTEdge.Models.MotorControl.Messages;
public class SetMotorSpeedForSecondsControlMessage : IMotorControlMessage
{
    public int Seconds { get; set; } = 0;
}
