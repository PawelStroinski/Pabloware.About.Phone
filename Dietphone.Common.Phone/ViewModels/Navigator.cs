using System;
using System.Windows.Navigation;
using System.Collections.Generic;
using Dietphone.Tools;
using Dietphone.Views;
using Pabloware.About;

namespace Dietphone.ViewModels
{
    public interface Navigator
    {
        void GoBack();
        void GoToMealEditing(Guid mealId);
        void GoToProductEditing(Guid productId);
        void GoToMain();
        void GoToMainToAddMealItem();
        void GoToAbout();
        void GoToExportAndImport();
        void GoToSettings();
        Guid GetMealIdToEdit();
        Guid GetProductIdToEdit();
        bool ShouldAddMealItem();
    }

    public enum Assembly { Default, Sometimes, Rarely };

    public class NavigatorImpl : Navigator
    {
        private string path;
        private string idName;
        private Guid idValue;
        private string action;
        private Assembly assembly;
        private readonly NavigationService service;
        private readonly IDictionary<string, string> passedQueryString;
        private readonly GoingToAbout about;
        private const string MEAL_ID_TO_EDIT = "MealIdToEdit";
        private const string PRODUCT_ID_TO_EDIT = "ProductIdToEdit";
        private const string ADD_MEAL_ITEM = "AddMealItem";
        private const string ABOUT_MAIL = "wp7@pabloware.com";
        private const string ABOUT_PATH_TO_LICENSE = "/Dietphone.Rarely.Phone;component/documents/license.{0}.txt";
        private const string ABOUT_CHANGELOG_URI = "http://www.pabloware.com/wp7/dietphone.changelog.{0}.xaml";
        private const string ABOUT_WEB = "http://www.pabloware.com/wp7";
        private const string ABOUT_PUBLISHER = "Pabloware";

        public NavigatorImpl(NavigationService service, NavigationContext context)
        {
            this.service = service;
            passedQueryString = context.QueryString;
            about = new GoingToAbout(service);
        }

        public void GoBack()
        {
            if (service.CanGoBack)
            {
                service.GoBack();
            }
        }

        public void GoToMealEditing(Guid mealId)
        {
            idName = MEAL_ID_TO_EDIT;
            idValue = mealId;
            path = "/Views/MealEditing.xaml";
            assembly = Assembly.Default;
            NavigateWithId();
        }

        public void GoToProductEditing(Guid productId)
        {
            idName = PRODUCT_ID_TO_EDIT;
            idValue = productId;
            path = "/Views/ProductEditing.xaml";
            assembly = Assembly.Sometimes;
            NavigateWithId();
        }

        public void GoToMain()
        {
            path = "/Views/Main.xaml";
            assembly = Assembly.Default;
            Navigate();
        }

        public void GoToMainToAddMealItem()
        {
            action = ADD_MEAL_ITEM;
            path = "/Views/Main.xaml";
            assembly = Assembly.Default;
            NavigateWithAction();
        }

        public void GoToAbout()
        {
            FillAboutDto();
            about.Go();
        }

        public void GoToExportAndImport()
        {
            path = "/Views/ExportAndImport.xaml";
            assembly = Assembly.Rarely;
            Navigate();
        }

        public void GoToSettings()
        {
            path = "/Views/Settings.xaml";
            assembly = Assembly.Rarely;
            Navigate();
        }

        public Guid GetMealIdToEdit()
        {
            idName = MEAL_ID_TO_EDIT;
            return GetId();
        }

        public Guid GetProductIdToEdit()
        {
            idName = PRODUCT_ID_TO_EDIT;
            return GetId();
        }

        public bool ShouldAddMealItem()
        {
            action = ADD_MEAL_ITEM;
            return GetAction();
        }

        private Guid GetId()
        {
            if (passedQueryString.ContainsKey(idName))
            {
                return new Guid(passedQueryString[idName]);
            }
            else
            {
                return Guid.Empty;
            }
        }

        private bool GetAction()
        {
            if (passedQueryString.ContainsKey(action))
            {
                return bool.Parse(passedQueryString[action]);
            }
            else
            {
                return false;
            }
        }

        private void Navigate()
        {
            var destination = new UriBuilder();
            destination.Path = path;
            Navigate(destination);
        }

        private void NavigateWithId()
        {
            var destination = new UriBuilder();
            destination.Path = path;
            destination.Query = String.Format("{0}={1}", idName, idValue);
            Navigate(destination);
        }

        private void NavigateWithAction()
        {
            var destination = new UriBuilder();
            destination.Path = path;
            destination.Query = String.Format("{0}={1}", action, true);
            Navigate(destination);
        }

        private void Navigate(UriBuilder destination)
        {
            destination.Scheme = string.Empty;
            destination.Host = GetAssemblyPrefix();
            var uri = new Uri(destination.ToString(), UriKind.Relative);
            service.Navigate(uri);
        }

        private string GetAssemblyPrefix()
        {
            switch (assembly)
            {
                case Assembly.Sometimes:
                    return "/Dietphone.Sometimes.Phone;component";
                case Assembly.Rarely:
                    return "/Dietphone.Rarely.Phone;component";
                default:
                    return string.Empty;
            }
        }

        private void FillAboutDto()
        {
            var dto = about.Dto;
            var appVersion = new AppVersion();
            dto.AppName = appVersion.GetAppName();
            dto.Version = appVersion.GetAppVersion();
            dto.Mail = ABOUT_MAIL;
            dto.Web = ABOUT_WEB;
            dto.Publisher = ABOUT_PUBLISHER;
            dto.PathToLicense = ABOUT_PATH_TO_LICENSE;
            dto.ChangelogUri = ABOUT_CHANGELOG_URI;
            dto.UiCulture = MyApp.CurrentUiCulture;
            dto.AboutAppLabel = Translations.AboutApp;
            dto.PublisherLabel = Translations.From;
            dto.VersionLabel = Translations.Version;
            dto.ReviewLabel = Translations.ReviewThisApp;
            dto.FeedbackLabel = Translations.Suggestions;
            dto.LicenseLabel = Translations.License;
            dto.WhatsNewLabel = Translations.WhatsNew;
            dto.WeInviteYouLabel = Translations.WeInviteYouToVisitThisPageForInformationAbout;
        }
    }
}