namespace NET.CityControl.IoTEdge.Models.LightControl;

internal class LedSettings
{
    public string LightUsbId { get; set; } = string.Empty;
    public int LedStripLength { get; set; } = 175;
    public string LightWebServiceUrl { get; set; } = String.Empty;
}
