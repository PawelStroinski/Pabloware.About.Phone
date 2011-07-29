using System;
using Dietphone.Models;
using System.ComponentModel;

namespace Dietphone.ViewModels
{
    public class ExportAndImportViewModel : ViewModelBase
    {
        public event EventHandler ExportComplete;
        public event EventHandler ImportSuccessful;
        public event EventHandler ErrorsDuringImport;
        public ExportAndImport Implementation { private get; set; }
        public string Data { get; set; }
        private bool isBusy;
        private bool errorsDuringImport;

        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            set
            {
                isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }

        public void Export()
        {
            if (IsBusy)
            {
                return;
            }
            var worker = new BackgroundWorker();
            worker.DoWork += delegate
            {
                Data = Implementation.Export();
            };
            worker.RunWorkerCompleted += delegate
            {
                IsBusy = false;
                OnExportComplete();
            };
            IsBusy = true;
            worker.RunWorkerAsync();
        }

        public void Import()
        {
            if (IsBusy)
            {
                return;
            }
            var worker = new BackgroundWorker();
            worker.DoWork += delegate
            {
                CatchedImport();
            };
            worker.RunWorkerCompleted += delegate
            {
                IsBusy = false;
                NotifyAfterImport();
            };
            IsBusy = true;
            errorsDuringImport = false;
            worker.RunWorkerAsync();
        }

        private void CatchedImport()
        {
            try
            {
                Implementation.Import(Data);
            }
            catch (Exception)
            {
                errorsDuringImport = true;
            }
        }

        private void NotifyAfterImport()
        {
            if (errorsDuringImport)
            {
                OnErrorsDuringImport();
            }
            else
            {
                OnImportSuccesful();
            }
        }

        protected void OnExportComplete()
        {
            if (ExportComplete != null)
            {
                ExportComplete(this, EventArgs.Empty);
            }
        }

        protected void OnErrorsDuringImport()
        {
            if (ErrorsDuringImport != null)
            {
                ErrorsDuringImport(this, EventArgs.Empty);
            }
        }

        protected void OnImportSuccesful()
        {
            if (ImportSuccessful != null)
            {
                ImportSuccessful(this, EventArgs.Empty);
            }
        }
    }
}
