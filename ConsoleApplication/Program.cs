using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ConsoleApplication
{
    static class Program
    {
        [STAThread]
        static void Main()
        {

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            using (var graphviewer = new GraphViewer( stopwatch) )
            {
                graphviewer.Show();
                Application.Run();
            }
           
        }
    }
}
