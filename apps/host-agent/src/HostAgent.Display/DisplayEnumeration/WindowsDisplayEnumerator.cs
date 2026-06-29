using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HostAgent.Display.DisplayEnumeration
{
    public sealed class WindowsDisplayEnumerator : IDisplayEnumerator
    {
        public Task<IReadOnlyList<DisplayInfo>> GetDisplaysAsync()
        {
            // Screen.AllScreens is a managed wrapper that returns all connected displays.
            // It includes bounds (size/position) and a primary flag.
            // [web:555]
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

                // Refresh rate is not directly exposed here; set 0 for now.
                const int refreshRate = 0;

                var isPrimary = screen.Primary;

                // Very naive virtual detection heuristic for now:
                // - Treat non-primary displays with certain keywords as virtual candidates.
                var deviceNameLower = deviceName.ToLowerInvariant();
                var isVirtualCandidate =
                    !isPrimary &&
                    (deviceNameLower.Contains("virtual") ||
                     deviceNameLower.Contains("vdd") ||
                     deviceNameLower.Contains("parsec"));

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