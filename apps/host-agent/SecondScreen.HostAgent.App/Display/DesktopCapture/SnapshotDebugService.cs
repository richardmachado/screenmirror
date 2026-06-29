using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SecondScreen.HostAgent.App.Display.DisplayEnumeration;

namespace SecondScreen.HostAgent.App.Display.DesktopCapture
{
    public sealed class SnapshotDebugService
    {
        private readonly IDisplayCaptureProvider _captureProvider;

        public SnapshotDebugService(IDisplayCaptureProvider captureProvider)
        {
            _captureProvider = captureProvider;
        }

        public async Task RunAsync(DisplayInfo display, CancellationToken ct)
        {
            var baseDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "SecondScreen",
                "Snapshots",
                Sanitize(display.DeviceName));

            Directory.CreateDirectory(baseDir);

            var i = 0;

            while (!ct.IsCancellationRequested)
            {
                var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_fff");
                var fileName = $"snapshot_{i:D4}_{timestamp}.png";
                var path = Path.Combine(baseDir, fileName);

                // This is the always-overwritten latest snapshot file.
                var latestPath = Path.Combine(baseDir, "latest.png");

                try
                {
                    // Capture into the timestamped file.
                    await _captureProvider.CaptureSnapshotAsync(display, path, ct);

                    // Also update latest.png so the API can serve it.
                    try
                    {
                        File.Copy(path, latestPath, overwrite: true);
                    }
                    catch
                    {
                        // Ignore errors updating latest; history is more important.
                    }
                }
                catch (Exception ex)
                {
                    var logPath = Path.Combine(baseDir, "snapshot-errors.txt");
                    await File.AppendAllTextAsync(
                        logPath,
                        $"{DateTime.UtcNow:u} Failed to capture snapshot: {ex}\r\n",
                        ct);
                }

                i++;

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), ct);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        private static string Sanitize(string input)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                input = input.Replace(c, '_');
            }

            return input;
        }
    }
}