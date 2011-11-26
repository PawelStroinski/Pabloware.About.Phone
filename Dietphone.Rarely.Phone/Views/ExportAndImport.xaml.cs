using System;
using System.Windows;
using Microsoft.Phone.Controls;
using Dietphone.ViewModels;
using System.Windows.Media;
using System.Windows.Input;
using Dietphone.Tools;

namespace Dietphone.Views
{
    public partial class ExportAndImport : PhoneApplicationPage
    {
        public ExportAndImportViewModel ViewModel { get; private set; }
        private bool exportMode;

        public ExportAndImport()
        {
            InitializeComponent();
            ViewModel = new ExportAndImportViewModel(MyApp.Factories);
            ViewModel.ExportAndSendSuccessful += ViewModel_ExportAndSendSuccessful;
            ViewModel.DownloadAndImportSuccessful += ViewModel_DownloadAndImportSuccessful;
            ViewModel.SendingFailedDuringExport += ViewModel_SendingFailedDuringExport;
            ViewModel.DownloadingFailedDuringImport += ViewModel_DownloadingFailedDuringImport;
            ViewModel.ReadingFailedDuringImport += ViewModel_ReadingFailedDuringImport;
            DataContext = ViewModel;
            SetWindowBackground();
            SetWindowSize();
            TranslateButtons();
        }

        private void ViewModel_ExportAndSendSuccessful(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                MessageBox.Show(Translations.ExportCompletedSuccessfully);
            });
        }

        private void ViewModel_DownloadAndImportSuccessful(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                MessageBox.Show(Translations.ImportCompletedSuccessfully);
            });
        }

        private void ViewModel_SendingFailedDuringExport(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                MessageBox.Show(Translations.AnErrorOccurredWhileSendingTheExportedData);
            });
        }

        private void ViewModel_DownloadingFailedDuringImport(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                MessageBox.Show(Translations.AnErrorOccurredWhileRetrievingTheImportedData);
            });
        }

        private void ViewModel_ReadingFailedDuringImport(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                MessageBox.Show(Translations.AnErrorOccurredDuringImport);
            });
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            exportMode = true;
            Info.Text = Translations.SendToAnEMailAddress;
            Input.Text = string.Empty;
            Input.InputScope = InputScopeNameValue.EmailSmtpAddress.GetInputScope();
            Window.IsOpen = true;
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            exportMode = false;
            Info.Text = Translations.DownloadFileFromAddress;
            Input.Text = "http://";
            Input.InputScope = InputScopeNameValue.Url.GetInputScope();
            Window.IsOpen = true;
        }

        private void WindowAnimation_Ended(object sender, EventArgs e)
        {
            if (Window.IsOpen)
            {
                Input.Focus();
                if (!exportMode)
                {
                    var text = Input.Text;
                    Input.Select(text.Length, 0);
                }
            }
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            if (exportMode)
            {
                ViewModel.Email = Input.Text;
                ViewModel.ExportAndSend();
            }
            else
            {
                ViewModel.Url = Input.Text;
                ViewModel.DownloadAndImport();
            }
            Window.IsOpen = false;
        }

        private void SetWindowBackground()
        {
            Color color;
            if (this.IsDarkTheme())
            {
                color = Color.FromArgb(0xCC, 0, 0, 0);
            }
            else
            {
                color = Color.FromArgb(0xCC, 255, 255, 255);
            }
            Window.Background = new SolidColorBrush(color);
        }

        private void SetWindowSize()
        {
            Loaded += (sender, args) =>
            {
                var size = new Size(Application.Current.RootVisual.RenderSize.Width, double.NaN);
                Window.WindowSize = size;
            };
        }

        private void TranslateButtons()
        {
            Export.Line1 = Translations.Export;
            Export.Line2 = Translations.AllowsSendingDataAttachedToAnEMail;
            Import.Line1 = Translations.Import;
            Import.Line2 = Translations.AllowsToRetrieveDataFromAFileInXmlFormat;
        }
    }
}