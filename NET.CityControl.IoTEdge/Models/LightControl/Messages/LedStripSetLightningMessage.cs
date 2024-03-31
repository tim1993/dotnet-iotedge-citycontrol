namespace NET.CityControl.IoTEdge.Models.LightControl.Messages;

public class LedStripSetLightningMessage: ILedStripBaseMessage
{
    public int R { get; set; } = 0;
    public int G { get; set; } = 0;
    public int B { get; set; } = 0;
    public int StartIndex { get; set; } = 0;
    public int Length { get; set; } = 0;
}

