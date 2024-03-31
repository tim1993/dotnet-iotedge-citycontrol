namespace NET.CityControl.IoTEdge.Models.LightControl.Messages;

internal static class LEDStripMessageType
{
    public const string 
        LedStripActionMessage = "LedStripActionMessage",
        LedStripResetMessage = "LedStripResetMessage",
        LedStripSetLightningMessage = "LedStripSetLightningMessage",
        SetLedStripLengthMessage = "SetLedStripLengthMessage";
}