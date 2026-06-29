using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SecondScreen.HostAgent.App.Display.DisplayEnumeration;

namespace SecondScreen.HostAgent.App.Display.DesktopCapture
{
    public sealed class CopyFromScreenCaptureProvider : IDisplayCaptureProvider
    {
        public Task CaptureSnapshotAsync(DisplayInfo display, string outputPath, CancellationToken ct)
        {
            if (display == null) throw new ArgumentNullException(nameof(display));
            if (outputPath == null) throw new ArgumentNullException(nameof(outputPath));

            // Map DisplayInfo.Id to the corresponding Screen.
            // We assume Id was the index of Screen.AllScreens.
            var screens = Screen.AllScreens;
            if (!int.TryParse(display.Id, out var idx) || idx < 0 || idx >= screens.Length)
            {
                throw new InvalidOperationException(
                    $"Display id '{display.Id}' does not map to an existing screen.");
            }

            var screen = screens[idx];
            var bounds = screen.Bounds;

            // Create a bitmap with the size of the selected display.
            using (var bitmap = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                // Capture the screen area corresponding to this display.
                graphics.CopyFromScreen(
                    sourceX: bounds.X,
                    sourceY: bounds.Y,
                    destinationX: 0,
                    destinationY: 0,
                    blockRegionSize: bounds.Size,
                    copyPixelOperation: CopyPixelOperation.SourceCopy);

                bitmap.Save(outputPath, ImageFormat.Png);
            }

            return Task.CompletedTask;
        }
    }
}