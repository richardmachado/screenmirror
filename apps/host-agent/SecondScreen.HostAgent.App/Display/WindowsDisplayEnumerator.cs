using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SecondScreen.HostAgent.App.Display.DisplayEnumeration
{
    public sealed class WindowsDisplayEnumerator : IDisplayEnumerator
    {
        public Task<IReadOnlyList<DisplayInfo>> GetDisplaysAsync()
        {
            var screens = Screen.AllScreens;

            var list = new List<DisplayInfo>();

            for (var i = 0; i < screens.Length; i++)
            {
                var screen = screens[i];

                var id = i.ToString();
                var friendlyName = $"Display {i + 1}";
                var deviceName = screen.DeviceName ?? $"DISPLAY{i + 1}";
                var width = screen.Bounds.Width;
                var height = screen.Bounds.Height;

                const int refreshRate = 0;

                var isPrimary = screen.Primary;

       var deviceNameLower = deviceName.ToLowerInvariant();

// Heuristic: treat non-primary displays with certain patterns as virtual.
var isVirtualCandidate =
    !isPrimary &&
    (
        deviceNameLower.Contains("display6") ||   // your current virtual display device
        deviceNameLower.Contains("virtual") ||
        deviceNameLower.Contains("vdd") ||
        deviceNameLower.Contains("parsec")
    );

                var info = new DisplayInfo(
                    id,
                    friendlyName,
                    deviceName,
                    width,
                    height,
                    refreshRate,
                    isPrimary,
                    isVirtualCandidate
                );

                list.Add(info);
            }

            return Task.FromResult<IReadOnlyList<DisplayInfo>>(list);
        }
    }
}