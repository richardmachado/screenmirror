using System.Collections.Generic;
using System.Linq;
using SecondScreen.HostAgent.App.Display.DisplayEnumeration;

namespace SecondScreen.HostAgent.App.Display.VirtualDisplayDetection
{
    public interface IVirtualDisplayDetector
    {
        DisplayInfo? ChooseVirtualDisplay(IReadOnlyList<DisplayInfo> displays);
    }

    public sealed class VirtualDisplayDetector : IVirtualDisplayDetector
    {
        public DisplayInfo? ChooseVirtualDisplay(IReadOnlyList<DisplayInfo> displays)
        {
            if (displays == null || displays.Count == 0)
            {
                return null;
            }

            var virtualCandidate = displays.FirstOrDefault(d => d.IsVirtualCandidate);
            if (virtualCandidate != null)
            {
                return virtualCandidate;
            }

            var nonPrimary = displays.FirstOrDefault(d => !d.IsPrimary);
            if (nonPrimary != null)
            {
                return nonPrimary;
            }

            return displays.First();
        }
    }
}