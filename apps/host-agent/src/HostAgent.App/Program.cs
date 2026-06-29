using System;
using System.Threading.Tasks;
using System.Windows;

namespace HostAgent.App
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            var app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}