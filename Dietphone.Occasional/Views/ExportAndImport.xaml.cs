using System;
using System.Windows;
using Microsoft.Phone.Controls;
using Dietphone.ViewModels;
using System.Windows.Media;

namespace Dietphone.Views
{
    public partial class ExportAndImport : PhoneApplicationPage
    {
        public ExportAndImportViewModel ViewModel { get; private set; }
        private bool copyMode;

        public ExportAndImport()
        {
            InitializeComponent();
            ViewModel = new ExportAndImportViewModel(MyApp.Factories);
            ViewModel.ExportComplete += ViewModel_ExportComplete;
            ViewModel.ImportSuccessful += ViewModel_ImportSuccessful;
            ViewModel.ErrorsDuringImport += ViewModel_ErrorsDuringImport;
            DataContext = ViewModel;
            SetWindowBackground();
            SetWindowSize();
        }

        private void ViewModel_ExportComplete(object sender, EventArgs e)
        {
            copyMode = true;
            CopyPasteInfo.Text = "Naciśnij ikonę kopiowania";
            OpenCopyPaste();
        }

        private void ViewModel_ImportSuccessful(object sender, EventArgs e)
        {
            MessageBox.Show("Operacja zakończona sukcesem.");
        }

        private void ViewModel_ErrorsDuringImport(object sender, EventArgs e)
        {
            MessageBox.Show("Wystąpił błąd podczas tej operacji. " +
                "Upewnij się, że wklejone dane nie są naruszone.");
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Export();
        }

        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            copyMode = false;
            CopyPasteInfo.Text = "Wklej tutaj";
            OpenCopyPaste();
        }

        private void CopyPasteAnimation_Ended(object sender, EventArgs e)
        {
            if (!CopyPaste.IsOpen)
            {
                return;
            }
            if (copyMode)
            {
                OpenedCopy();
            }
            else
            {
                OpenedPaste();
            }
        }

        private void CopyPasteClose_Click(object sender, RoutedEventArgs e)
        {
            if (copyMode)
            {
                CloseCopy();
            }
            else
            {
                ClosePaste();
            }
        }

        private void OpenCopyPaste()
        {
            CopyPasteText.Text = string.Empty;
            CopyPaste.IsOpen = true;
        }

        private void OpenedCopy()
        {
            CopyPasteText.Text = ViewModel.Data;
            CopyPasteText.Focus();
            CopyPasteText.SelectAll();
        }

        private void OpenedPaste()
        {
            CopyPasteText.Focus();
        }

        private void CloseCopy()
        {
            CopyPasteText.Text = string.Empty;
            Dispatcher.BeginInvoke(() =>
            {
                CopyPaste.IsOpen = false;
            });
        }

        private void ClosePaste()
        {
            ViewModel.Data = CopyPasteText.Text;
            CopyPasteText.Text = string.Empty;
            Dispatcher.BeginInvoke(() =>
            {
                CopyPaste.IsOpen = false;
                ViewModel.Import();
            });
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
            CopyPaste.Background = new SolidColorBrush(color);
        }

        private void SetWindowSize()
        {
            Loaded += (sender, args) =>
            {
                var size = new Size(Application.Current.RootVisual.RenderSize.Width, double.NaN);
                CopyPaste.WindowSize = size;
            };
        }

        private bool IsDarkTheme()
        {
            return (Visibility)Resources["PhoneDarkThemeVisibility"] == Visibility.Visible;
        }
    }
}