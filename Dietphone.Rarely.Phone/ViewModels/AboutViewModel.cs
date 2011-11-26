// Kod LoadLicense() zaczerpnięty z http://www.jeff.wilcox.name/2011/07/my-app-about-page/
using System.Windows.Controls;
using System.Reflection;
using Microsoft.Phone.Tasks;
using System.Windows.Resources;
using System.Windows;
using System;
using System.IO;
using System.Windows.Shapes;
using Dietphone.Tools;
using Dietphone.Views;

namespace Dietphone.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public StackPanel License { get; private set; }
        private byte selectedPivot = DEFAULT_PIVOT;
        private readonly OptionalDispatcher dispatcher;
        private readonly ResourceStreamProvider resStreamProvider;
        private readonly AppVersion appVersion = new AppVersion();
        private const string MAIL = "dietphone@pabloware.com";
        private const byte LICENSE_PIVOT = 1;
        private const byte DEFAULT_PIVOT = 0;
        private const string PATH_TO_LICENSE = "/Dietphone.Rarely.Phone;component/documents/license.{0}.txt";
        private const string CHANGELOG_URI = "http://www.pabloware.com/dietphone/changelog.{0}.xaml";

        public AboutViewModel(OptionalDispatcher dispatcher, ResourceStreamProvider resStreamProvider)
        {
            this.dispatcher = dispatcher;
            this.resStreamProvider = resStreamProvider;
        }

        public string AppName
        {
            get
            {
                return appVersion.GetAppName();
            }
        }

        public string AppVersion
        {
            get
            {
                var version = appVersion.GetAppVersion();
                return string.Format(Translations.Version, version);
            }
        }

        public string ChangelogUri
        {
            get
            {
                return string.Format(CHANGELOG_URI, MyApp.CurrentUiCulture);
            }
        }

        public byte SelectedPivot
        {
            get
            {
                OnSelectedPivotChange();
                return selectedPivot;
            }
            set
            {
                selectedPivot = value;
                OnSelectedPivotChange();
            }
        }

        public void OpenReview()
        {
            var task = new MarketplaceReviewTask();
            task.Show();
        }

        public void OpenFeedback()
        {
            EmailComposeTask task = new EmailComposeTask();
            task.To = MAIL;
            task.Show();
        }


        private void OnSelectedPivotChange()
        {
            if (selectedPivot == LICENSE_PIVOT)
            {
                LazyLoadLicense();
            }
        }

        private void LazyLoadLicense()
        {
            if (License == null)
            {
                dispatcher.BeginInvoke(() =>
                {
                    LoadLicense();
                    OnPropertyChanged("License");
                });
            }
        }

        private void LoadLicense()
        {
            License = new StackPanel();
            var children = License.Children;
            var pathToLicense = string.Format(PATH_TO_LICENSE, MyApp.CurrentUiCulture);
            var stream = resStreamProvider.GetResourceStream(pathToLicense);
            using (var reader = new StreamReader(stream))
            {
                string line = null;
                var lastWasEmpty = true;
                do
                {
                    line = reader.ReadLine();
                    if (line == string.Empty)
                    {
                        var rectangle = MakeRectangle();
                        children.Add(rectangle);
                        lastWasEmpty = true;
                    }
                    else
                    {
                        var textBlock = MakeTextBlock();
                        textBlock.Text = line;
                        if (!lastWasEmpty)
                        {
                            textBlock.Opacity = 0.7;
                        }
                        lastWasEmpty = false;
                        children.Add(textBlock);
                    }
                } while (line != null);
            }
        }

        private Rectangle MakeRectangle()
        {
            return new Rectangle
            {
                Height = 20
            };
        }

        private TextBlock MakeTextBlock()
        {
            return new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Style = GetPhoneTextNormalStyle()
            };
        }

        private Style GetPhoneTextNormalStyle()
        {
            var application = Application.Current;
            if (application != null)
            {
                return (Style)application.Resources["PhoneTextNormalStyle"];
            }
            else
            {
                return null;
            }
        }
    }
}
