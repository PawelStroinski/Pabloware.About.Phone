using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Phone.Tasks;

namespace Dietphone.ViewModels
{
    public class AboutViewModel
    {
        private const byte NAME_PART_NUMBER = 0;
        private const byte VERSION_PART_NUMBER = 1;
        private const string USELESS_MINOR_VERSION = ".0.0";
        private const string URL = "http://www.pabloware.com/dietphone";
        private const string MAIL = "dietphone@pabloware.com";

        public string AppNameUppercased
        {
            get
            {
                return AppName.ToUpper();
            }
        }

        public string AppName
        {
            get
            {
                var name = GetPartOfAssemblyName(NAME_PART_NUMBER);
                var dotParts = name.Split('.');
                return dotParts[0];
            }
        }

        public string AppVersion
        {
            get
            {
                return string.Format("Wersja: {0}", GetAppVersion());
            }
        }

        public void OpenWeb()
        {
            var task = new WebBrowserTask();
            task.URL = URL;
            task.Show();
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

        private string GetAppVersion()
        {
            var version = GetPartOfAssemblyName(VERSION_PART_NUMBER);
            var equationParts = version.Split('=');
            var numbers = equationParts[1];
            if (numbers.EndsWith(USELESS_MINOR_VERSION))
            {
                numbers = numbers.Remove(numbers.Length -
                    USELESS_MINOR_VERSION.Length, USELESS_MINOR_VERSION.Length);
            }
            return numbers;
        }

        private string GetPartOfAssemblyName(byte partNumber)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var name = assembly.FullName;
            var parts = name.Split(',');
            return parts[partNumber];
        }
    }
}
