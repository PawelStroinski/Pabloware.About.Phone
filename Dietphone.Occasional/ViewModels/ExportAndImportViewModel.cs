using System;
using Dietphone.Models;
using System.ComponentModel;
using Dietphone.Tools;
using System.Net;
using System.Text;
using System.Windows;

namespace Dietphone.ViewModels
{
    public class ExportAndImportViewModel : ViewModelBase
    {
        public event EventHandler ExportAndSendSuccessful;
        public event EventHandler DownloadAndImportSuccessful;
        public event EventHandler SendingFailedDuringExport;
        public event EventHandler DownloadingFailedDuringImport;
        public event EventHandler ReadingFailedDuringImport;
        public string Email { private get; set; }
        public string Url { private get; set; }
        private string data;
        private bool isBusy;
        private bool readingFailedDuringImport;
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
                OnPropertyChanged("IsBusyAsVisibility");
            }
        }

        public Visibility IsBusyAsVisibility
        {
            get
            {
                return IsBusy.ToVisibility();
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
            var worker = new VerboseBackgroundWorker();
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

        public void DownloadAndImport()
        {
            if (IsBusy)
            {
                return;
            }
            Download();
        }

        private void Send()
        {
            var sender = new PostSender(MAILEXPORT_URL);
            sender.Inputs["address"] = Email;
            sender.Inputs["data"] = data;
            sender.Completed += Send_Completed;
            sender.SendAsync();
        }

        private void Download()
        {
            if (!Url.IsValidUri())
            {
                OnDownloadingFailedDuringImport();
                return;
            }
            IsBusy = true;
            var web = new WebClient();
            web.Encoding = Encoding.Unicode;
            web.DownloadStringCompleted += Download_Completed;
            web.DownloadStringAsync(new Uri(Url));
        }

        private void Send_Completed(object sender, UploadStringCompletedEventArgs e)
        {
            IsBusy = false;
            if (e.IsGeneralSuccess() && e.Result == MAILEXPORT_SUCCESS_RESULT)
            {
                OnExportAndSendSuccessful();
            }
            else
            {
                OnSendingFailedDuringExport();
            }
        }

        private void Download_Completed(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.IsGeneralSuccess())
            {
                data = e.Result;
                Import();
            }
            else
            {
                IsBusy = false;
                OnDownloadingFailedDuringImport();
            }
        }

        private void Import()
        {
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
            readingFailedDuringImport = false;
            worker.RunWorkerAsync();
        }

        private void CatchedImport()
        {
            try
            {
                exportAndImport.Import(data);
            }
            catch (Exception)
            {
                readingFailedDuringImport = true;
            }
        }

        private void NotifyAfterImport()
        {
            if (readingFailedDuringImport)
            {
                OnReadingFailedDuringImport();
            }
            else
            {
                OnDownloadAndImportSuccesful();
            }
        }

        protected void OnExportAndSendSuccessful()
        {
            if (ExportAndSendSuccessful != null)
            {
                ExportAndSendSuccessful(this, EventArgs.Empty);
            }
        }

        protected void OnDownloadAndImportSuccesful()
        {
            if (DownloadAndImportSuccessful != null)
            {
                DownloadAndImportSuccessful(this, EventArgs.Empty);
            }
        }

        protected void OnSendingFailedDuringExport()
        {
            if (SendingFailedDuringExport != null)
            {
                SendingFailedDuringExport(this, EventArgs.Empty);
            }
        }

        protected void OnDownloadingFailedDuringImport()
        {
            if (DownloadingFailedDuringImport != null)
            {
                DownloadingFailedDuringImport(this, EventArgs.Empty);
            }
        }

        protected void OnReadingFailedDuringImport()
        {
            if (ReadingFailedDuringImport != null)
            {
                ReadingFailedDuringImport(this, EventArgs.Empty);
            }
        }
    }
}
