namespace NET.CityControl.IoTEdge.Models.LightControl.Messages;

public class LedStripActionMessage: ILedStripBaseMessage
{
    public LedStripActions LedStripAction { get; set; }
}
