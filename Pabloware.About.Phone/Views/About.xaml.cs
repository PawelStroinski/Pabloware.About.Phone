using System.Windows;
using Pabloware.About.ViewModels;
using Pabloware.About.Tools;
using System.Windows.Navigation;

namespace Pabloware.About.Views
{
    public partial class About : StateProviderPage
    {
        private AboutViewModel viewModel;
        private double licenseScrollPos;
        private double changelogScrollPos;
        private const string LICENSE_SCROLL_POS = "LICENSE_SCROLL_POS";
        private const string CHANGELOG_SCROLL_POS = "CHANGELOG_SCROLL_POS";

        public About()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var dispatcher = new OptionalDispatcher(Dispatcher);
            var resStreamProvider = new PhoneResourceStreamProvider();
            var coming = new ComingToAbout(NavigationContext);
            viewModel = new AboutViewModel(dispatcher, resStreamProvider);
            viewModel.Coming = coming;
            viewModel.StateProvider = this;
            viewModel.Untombstone();
            Untombstone();
            DataContext = viewModel;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode != NavigationMode.Back)
            {
                viewModel.Tombstone();
                Tombstone();
            }
        }

        private void Review_Click(object sender, RoutedEventArgs e)
        {
            viewModel.OpenReview();
        }

        private void Feddback_Click(object sender, RoutedEventArgs e)
        {
            viewModel.OpenFeedback();
        }

        private void LicenseControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RestoreLicenseScrollPos();
        }

        private void LicenseControl_Loaded(object sender, RoutedEventArgs e)
        {
            RestoreLicenseScrollPos();
        }

        private void ChangelogControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RestoreChangelogScrollPos();
        }

        private void Tombstone()
        {
            State[LICENSE_SCROLL_POS] = License.VerticalOffset;
            State[CHANGELOG_SCROLL_POS] = Changelog.VerticalOffset;
        }

        private void Untombstone()
        {
            if (State.ContainsKey(LICENSE_SCROLL_POS))
            {
                licenseScrollPos = (double)State[LICENSE_SCROLL_POS];
                changelogScrollPos = (double)State[CHANGELOG_SCROLL_POS];
            }
        }

        private void RestoreLicenseScrollPos()
        {
            if (licenseScrollPos != 0)
            {
                License.ScrollToVerticalOffset(licenseScrollPos);
            }
        }

        private void RestoreChangelogScrollPos()
        {
            if (changelogScrollPos != 0)
            {
                Changelog.ScrollToVerticalOffset(changelogScrollPos);
            }
        }
    }
}