using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using HostAgent.Display.DisplayEnumeration;
using HostAgent.Display.VirtualDisplayDetection;

namespace HostAgent.App
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // In a real app you would use DI; here we compose manually.
            var enumerator = new WindowsDisplayEnumerator();
            var detector = new VirtualDisplayDetector();

            // Fire and forget for now; add proper error handling later.
            _ = Task.Run(async () =>
            {
                try
                {
                    var displays = await enumerator.GetDisplaysAsync();

                    var chosen = detector.ChooseVirtualDisplay(displays);

                    var logDir = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                        "SecondScreen", "Logs");

                    Directory.CreateDirectory(logDir);

                    var logPath = Path.Combine(logDir, "displays.txt");

                    using var writer = new StreamWriter(logPath, append: false);
                    writer.WriteLine($"Enumerated {displays.Count} displays at {DateTime.UtcNow:u}");

                    foreach (var d in displays)
                    {
                        writer.WriteLine(d.ToString());
                    }

                    writer.WriteLine();
                    writer.WriteLine("Chosen display:");
                    writer.WriteLine(chosen?.ToString() ?? "None");

                    await writer.FlushAsync();
                }
                catch (Exception ex)
                {
                    // Minimal crash logging for now.
                    var logDir = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                        "SecondScreen", "Logs");
                    Directory.CreateDirectory(logDir);
                    var logPath = Path.Combine(logDir, "startup-error.txt");
                    File.WriteAllText(logPath, ex.ToString());
                }
            });
        }
    }
}