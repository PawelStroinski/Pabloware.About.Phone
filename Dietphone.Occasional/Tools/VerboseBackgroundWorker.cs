using System.ComponentModel;

namespace Dietphone.Tools
{
    public class VerboseBackgroundWorker : BackgroundWorker
    {
        protected override void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                throw e.Error;
            }
            base.OnRunWorkerCompleted(e);
        }
    }
}
