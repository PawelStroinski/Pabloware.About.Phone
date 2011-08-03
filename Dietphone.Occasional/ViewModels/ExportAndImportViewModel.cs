using System;
using Dietphone.Models;
using System.ComponentModel;
using Dietphone.Tools;

namespace Dietphone.ViewModels
{
    public class ExportAndImportViewModel : ViewModelBase
    {
        public event EventHandler ExportAndSendSuccessful;
        public event EventHandler ImportSuccessful;
        public event EventHandler SendingFailedDuringExport;
        public event EventHandler ErrorsDuringImport;
        public string Email { private get; set; }
        public string Url { private get; set; }
        private string data;
        private bool isBusy;
        private bool errorsDuringImport;
        private readonly ExportAndImport exportAndImport;
        private const string MAILEXPORT_URL = "http://bizmaster.pl/varia/dietphone/MailExport.aspx";
        private const string MAILEXPORT_SUCCESS_RESULT = "Success!";

        public ExportAndImportViewModel(Factories factories)
        {
            exportAndImport = new ExportAndImport(factories);
        }

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

        public void ExportAndSend()
        {
            if (IsBusy)
            {
                return;
            }
            if (!Email.IsValidEmail())
            {
                OnSendingFailedDuringExport();
                return;
            }
            var worker = new BackgroundWorker();
            worker.DoWork += delegate
            {
                data = exportAndImport.Export();
            };
            worker.RunWorkerCompleted += delegate
            {
                Send();
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

        private void Send()
        {
            var sender = new PostSender(MAILEXPORT_URL);
            sender.Inputs["address"] = Email;
            sender.Inputs["data"] = data;
            sender.Completed += Send_Completed;
            sender.SendAsync();
        }

        private void Send_Completed(object sender, System.Net.UploadStringCompletedEventArgs e)
        {
            if (e.Error == null && e.Result == MAILEXPORT_SUCCESS_RESULT)
            {
                OnExportAndSendSuccessful();
            }
            else
            {
                OnSendingFailedDuringExport();
            }
            IsBusy = false;
        }

        private void CatchedImport()
        {
            try
            {
                exportAndImport.Import(data);
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

        protected void OnExportAndSendSuccessful()
        {
            if (ExportAndSendSuccessful != null)
            {
                ExportAndSendSuccessful(this, EventArgs.Empty);
            }
        }

        protected void OnImportSuccesful()
        {
            if (ImportSuccessful != null)
            {
                ImportSuccessful(this, EventArgs.Empty);
            }
        }

        protected void OnSendingFailedDuringExport()
        {
            if (SendingFailedDuringExport != null)
            {
                SendingFailedDuringExport(this, EventArgs.Empty);
            }
        }

        protected void OnErrorsDuringImport()
        {
            if (ErrorsDuringImport != null)
            {
                ErrorsDuringImport(this, EventArgs.Empty);
            }
        }
    }
}
