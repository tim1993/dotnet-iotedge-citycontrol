using Iot.Device.Ws28xx;
using System.Drawing;

namespace NET.CityControl.IoTEdge.Services.LightControl;

public static class LedStripActionHandlers
{
    /// <summary>
    /// Knight Rider LED action
    /// </summary>
    /// <param name="ledDevice"></param>
    /// <param name="numberOfLeds"></param>
    /// <param name="token"></param>
    public static async Task KnightRiderAsync(Ws2812b ledDevice, int numberOfLeds, CancellationToken cancellationToken)
    {
        var img = ledDevice.Image;
        var downDirection = false;

        var beamLength = 15;

        var index = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            for (int i = 0; i < numberOfLeds; i++)
            {
                img.SetPixel(i, 0, Color.FromArgb(0, 0, 0, 0));
            }

            if (downDirection)
            {
                for (int i = 0; i <= beamLength; i++)
                {
                    if (index + i < numberOfLeds && index + i >= 0)
                    {
                        var redValue = (beamLength - i) * (255 / (beamLength + 1));
                        img.SetPixel(index + i, 0, Color.FromArgb(0, redValue, 0, 0));
                    }
                }

                index--;
                if (index < -beamLength)
                {
                    downDirection = false;
                    index = 0;
                }
            }
            else
            {
                for (int i = beamLength - 1; i >= 0; i--)
                {
                    if (index - i >= 0 && index - i < numberOfLeds)
                    {
                        var redValue = (beamLength - i) * (255 / (beamLength + 1));
                        img.SetPixel(index - i, 0, Color.FromArgb(0, redValue, 0, 0));
                    }
                }

                index++;
                if (index - beamLength >= numberOfLeds)
                {
                    downDirection = true;
                    index = numberOfLeds - 1;
                }
            }

            ledDevice.Update();
            await Task.Delay(10, default).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Rainbows the specified count.
    /// </summary>
    /// <param name="token">The token.</param>
    public static async Task RainbowAsync(Ws2812b ledDevice, int numberOfLeds, CancellationToken cancellationToken)
    {
        RawPixelContainer img = ledDevice.Image;
        while (!cancellationToken.IsCancellationRequested)
        {
            for (var i = 0; i < 255; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                for (var j = 0; j < numberOfLeds; j++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    img.SetPixel(j, 0, Wheel((i + j) & 255));
                }

                ledDevice.Update();
                await Task.Delay(25, default).ConfigureAwait(false);
            }
        }
    }

    private static Color Wheel(int position)
    {
        if (position < 85)
        {
            return Color.FromArgb(0, position * 3, 255 - position * 3, 0);
        }
        else if (position < 170)
        {
            position -= 85;
            return Color.FromArgb(0, 255 - position * 3, 0, position * 3);
        }
        else
        {
            position -= 170;
            return Color.FromArgb(0, 0, position * 3, 255 - position * 3);
        }
    }
}

