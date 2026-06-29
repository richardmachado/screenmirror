using System;
using System.Threading;
using System.Threading.Tasks;
using DesktopDuplicationWapper;
using SecondScreen.HostAgent.App.Display.DisplayEnumeration;

namespace SecondScreen.HostAgent.App.Display.DesktopCapture
{
    public sealed class DesktopDuplicationCaptureProvider : IDisplayCaptureProvider, IDisposable
    {
        public async Task CaptureSnapshotAsync(DisplayInfo display, string outputPath, CancellationToken ct)
        {
            if (display == null) throw new ArgumentNullException(nameof(display));
            if (outputPath == null) throw new ArgumentNullException(nameof(outputPath));

            // TEMP: hard-code adapter 0, output 0 to match the sample exactly.
            const int adapterIndex = 0;
            const int outputIndex = 0;

            using (var desktopDuplicator = new DesktopDuplicator(adapterIndex, outputIndex))
            {
                // TEMP: use a conservative size until we confirm correct behavior.
                // You can adjust these once you see clean frames.
                var frame = desktopDuplicator.GetLatestFrame(
                    x: 0,
                    y: 0,
                    width: 800,
                    height: 600);

                if (frame == null)
                {
                    return;
                }

                await Task.Run(() => frame.DesktopImage.Save(outputPath), ct);
            }
        }

        public void Dispose()
        {
        }
    }
}