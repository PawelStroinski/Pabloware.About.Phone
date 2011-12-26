// Kod LoadLicense() zaczerpnięty z http://www.jeff.wilcox.name/2011/07/my-app-about-page/
using System.Windows.Controls;
using Microsoft.Phone.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Shapes;
using Pabloware.About.Tools;
using Pabloware.About.Views;
using System;

namespace Pabloware.About.ViewModels
{
    public class AboutViewModel : PivotTombstoningViewModel
    {
        public StackPanel License { get; private set; }
        private int pivot = DEFAULT_PIVOT;
        private AboutDto dto;
        private readonly OptionalDispatcher dispatcher;
        private readonly ResourceStreamProvider resStreamProvider;
        private const int LICENSE_PIVOT = 1;
        private const int DEFAULT_PIVOT = 0;

        internal AboutViewModel(OptionalDispatcher dispatcher, ResourceStreamProvider resStreamProvider)
        {
            this.dispatcher = dispatcher;
            this.resStreamProvider = resStreamProvider;
        }

        internal ComingToAbout Coming
        {
            set
            {
                dto = value.Dto;
            }
        }

        public string AppNameUppercase
        {
            get
            {
                return AppName.ToUpper();
            }
        }

        public string AboutAppLabel
        {
            get
            {
                return dto.AboutAppLabel;
            }
        }

        public string AppName
        {
            get
            {
                return dto.AppName;
            }
        }

        public string Publisher
        {
            get
            {
                return string.Format(dto.PublisherLabel, dto.Publisher);
            }
        }

        public string Web
        {
            get
            {
                return dto.Web;
            }
        }

        public string WebLabel
        {
            get
            {
                return Web.Replace("http://", string.Empty);
            }
        }

        public string Version
        {
            get
            {
                return string.Format(dto.VersionLabel, dto.Version);
            }
        }

        public string ReviewLabel
        {
            get
            {
                return dto.ReviewLabel;
            }
        }

        public string FeedbackLabel
        {
            get
            {
                return string.Format(dto.FeedbackLabel, dto.Mail);
            }
        }

        public string LicenseLabel
        {
            get
            {
                return dto.LicenseLabel;
            }
        }

        public string WhatsNewLabel
        {
            get
            {
                return dto.WhatsNewLabel;
            }
        }

        public string ChangelogUri
        {
            get
            {
                return string.Format(dto.ChangelogUri, dto.UiCulture);
            }
        }


        public string WeInviteYouLabel
        {
            get
            {
                return dto.WeInviteYouLabel;
            }
        }

        public override int Pivot
        {
            get
            {
                OnPivotMayChanged();
                return pivot;
            }
            set
            {
                pivot = value;
                OnPivotMayChanged();
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
            task.To = dto.Mail;
            task.Show();
        }

        private void OnPivotMayChanged()
        {
            if (pivot == LICENSE_PIVOT)
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
            var pathToLicense = string.Format(dto.PathToLicense, dto.UiCulture);
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
