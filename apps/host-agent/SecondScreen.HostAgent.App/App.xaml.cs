using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using SecondScreen.HostAgent.App.Display.DesktopCapture;
using SecondScreen.HostAgent.App.Display.DisplayEnumeration;
using SecondScreen.HostAgent.App.Display.VirtualDisplayDetection;

namespace SecondScreen.HostAgent.App
{
    public partial class App : Application
    {
        private CancellationTokenSource? _cts;

   protected override void OnStartup(StartupEventArgs e)
{
    base.OnStartup(e);

    var enumerator = new WindowsDisplayEnumerator();
    var detector = new VirtualDisplayDetector();
   var captureProvider = new CopyFromScreenCaptureProvider(); // swap here
    var snapshotService = new SnapshotDebugService(captureProvider);

    _cts = new CancellationTokenSource();

    _ = Task.Run(async () =>
    {
        try
        {
            var displays = await enumerator.GetDisplaysAsync();
            var chosen = detector.ChooseVirtualDisplay(displays);

            // existing logging code...

            if (chosen != null)
            {
                await snapshotService.RunAsync(chosen, _cts.Token);
            }
        }
        catch (Exception ex)
        {
            // existing error logging...
        }
    });
}
        protected override void OnExit(ExitEventArgs e)
        {
            _cts?.Cancel();
            base.OnExit(e);
        }
    }
}