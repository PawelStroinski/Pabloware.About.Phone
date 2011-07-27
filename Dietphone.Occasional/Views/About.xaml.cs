using System.Windows;
using Microsoft.Phone.Controls;
using Dietphone.ViewModels;
using Dietphone.Tools;

namespace Dietphone.Views
{
    public partial class About : PhoneApplicationPage
    {
        private AboutViewModel viewModel;

        public About()
        {
            InitializeComponent();
            var dispatcher = new OptionalDispatcher(Dispatcher);
            var resStreamProvider = new PhoneResourceStreamProvider();
            viewModel = new AboutViewModel(dispatcher, resStreamProvider);
            DataContext = viewModel;
        }

        private void Review_Click(object sender, RoutedEventArgs e)
        {
            viewModel.OpenReview();
        }

        private void Feddback_Click(object sender, RoutedEventArgs e)
        {
            viewModel.OpenFeedback();
        }
    }
}