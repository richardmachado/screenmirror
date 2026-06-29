using System;
using System.Collections.Generic;
using System.Linq;
using HostAgent.Display.DisplayEnumeration;

namespace HostAgent.Display.VirtualDisplayDetection
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

            // 1. Prefer anything explicitly marked as a virtual candidate.
            var virtualCandidate = displays.FirstOrDefault(d => d.IsVirtualCandidate);
            if (virtualCandidate != null)
            {
                return virtualCandidate;
            }

            // 2. Otherwise prefer a non-primary display.
            var nonPrimary = displays.FirstOrDefault(d => !d.IsPrimary);
            if (nonPrimary != null)
            {
                return nonPrimary;
            }

            // 3. As a last resort, fall back to the primary display.
            //     (We will log this and treat it as a misconfiguration.)
            return displays.First();
        }
    }
}