namespace NET.CityControl.IoTEdge.Models.LightControl.Messages;

public class SetLedStripLengthMessage: ILedStripBaseMessage
{
    public uint NumberOfLeds { get; set; } = 0;
}

