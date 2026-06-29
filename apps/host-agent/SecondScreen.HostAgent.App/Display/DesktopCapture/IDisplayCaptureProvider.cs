using System.Threading;
using System.Threading.Tasks;
using SecondScreen.HostAgent.App.Display.DisplayEnumeration;

namespace SecondScreen.HostAgent.App.Display.DesktopCapture
{
    public interface IDisplayCaptureProvider
    {
        Task CaptureSnapshotAsync(DisplayInfo display, string outputPath, CancellationToken ct);
    }
}