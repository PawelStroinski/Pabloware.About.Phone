using System;
using System.Windows;
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
            try
            {
                Show();
            }
            catch (Exception)
            {
            }
        }

        public static void Show()
        {
            var elapsed = watch.ElapsedMilliseconds;
            if (elapsed != 0)
            {
                MessageBox.Show(elapsed.ToString());
            }
        }
    }
}
