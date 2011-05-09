using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace Dietphone.Tools
{
    public static class MyStopwatch
    {
        private static Stopwatch watch = new Stopwatch();

        public static void Start()
        {
            watch.Reset();
            watch.Start();
        }

        public static void Stop()
        {
            watch.Stop();
            var elapsed = watch.ElapsedMilliseconds;
            if (elapsed != 0)
            {
                MessageBox.Show(elapsed.ToString());
            }
        }
    }
}
