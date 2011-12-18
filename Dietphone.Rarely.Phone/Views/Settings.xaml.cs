using System.Windows;
using Dietphone.ViewModels;
using System.Windows.Navigation;

namespace Dietphone.Views
{
    public partial class Settings : StateProviderPage
    {
        private SettingsViewModel viewModel;

        public Settings()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            viewModel = new SettingsViewModel(MyApp.Factories);
            viewModel.StateProvider = this;
            viewModel.Untombstone();
            DataContext = viewModel;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode != NavigationMode.Back)
            {
                viewModel.Tombstone();
            }
        }

        private void LearnCuAndFpu_Click(object sender, RoutedEventArgs e)
        {
            var learn = new LearningCuAndFpu();
            learn.LearnCuAndFpu();
        }
    }
}