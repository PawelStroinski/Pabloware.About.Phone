using System.Windows;
using Microsoft.Phone.Controls;
using Dietphone.ViewModels;

namespace Dietphone.Views
{
    public partial class Settings : PhoneApplicationPage
    {
        private SettingsViewModel viewModel;

        public Settings()
        {
            InitializeComponent();
            viewModel = new SettingsViewModel(MyApp.Factories);
            DataContext = viewModel;
        }

        private void LearnCuAndFpu_Click(object sender, RoutedEventArgs e)
        {
            var learn = new LearningCuAndFpu();
            learn.LearnCuAndFpu();
        }
    }
}