using NET.CityControl.IoTEdge.Models.LightControl;
using System.Drawing;

namespace NET.CityControl.IoTEdge.Core.Extensions;

internal static class ColorValueExtensions
{
    public static Color ToColor(this ColorValue colorValue)
    {
        if (colorValue is null)
            return Color.Black;

        return Color.FromArgb(colorValue.A, colorValue.R, colorValue.G, colorValue.B);
    }
}
