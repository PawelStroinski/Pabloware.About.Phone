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
            ViewModel.ImportSuccessful += ViewModel_ImportSuccessful;
            ViewModel.SendingFailedDuringExport += ViewModel_SendingFailedDuringExport;
            ViewModel.ErrorsDuringImport += ViewModel_ErrorsDuringImport;
            DataContext = ViewModel;
            SetWindowBackground();
            SetWindowSize();
        }

        private void ViewModel_ExportAndSendSuccessful(object sender, EventArgs e)
        {
            MessageBox.Show("Eksport zakończony sukcesem.");
        }

        private void ViewModel_ImportSuccessful(object sender, EventArgs e)
        {
            MessageBox.Show("Import zakończony sukcesem.");
        }

        private void ViewModel_SendingFailedDuringExport(object sender, EventArgs e)
        {
            MessageBox.Show("Wystąpił błąd podczas wysyłania eksportowanych danych. " +
                "Upewnij się, że masz dostęp do internetu i adres e-mail jest prawidłowy.");
        }

        private void ViewModel_ErrorsDuringImport(object sender, EventArgs e)
        {
            MessageBox.Show("Wystąpił błąd podczas importu. " +
                "Upewnij się, że importowane dane nie były naruszone.");
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            exportMode = true;
            Info.Text = "Adres e-mail do wysłania";
            Input.Text = string.Empty;
            Input.InputScope = InputScopeNameValue.EmailSmtpAddress.GetInputScope();
            Window.IsOpen = true;
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            exportMode = false;
            Info.Text = "Adres pliku do pobrania";
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
            }
            Window.IsOpen = false;
        }

        private void SetWindowBackground()
        {
            Color color;
            if (IsDarkTheme())
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

        private bool IsDarkTheme()
        {
            return (Visibility)Resources["PhoneDarkThemeVisibility"] == Visibility.Visible;
        }
    }
}