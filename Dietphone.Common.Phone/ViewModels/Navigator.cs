using System;
using System.Windows.Navigation;
using System.Collections.Generic;

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

    public enum Assembly { Default, Occasional, Medium };

    public class NavigatorImpl : Navigator
    {
        private string path;
        private string idName;
        private Guid idValue;
        private string action;
        private Assembly assembly;
        private readonly NavigationService service;
        private readonly IDictionary<string, string> passedQueryString;
        private const string MEAL_ID_TO_EDIT = "MealIdToEdit";
        private const string PRODUCT_ID_TO_EDIT = "ProductIdToEdit";
        private const string ADD_MEAL_ITEM = "AddMealItem";

        public NavigatorImpl(NavigationService service, NavigationContext context)
        {
            this.service = service;
            passedQueryString = context.QueryString;
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
            assembly = Assembly.Medium;
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
            path = "/Views/About.xaml";
            assembly = Assembly.Occasional;
            Navigate();
        }

        public void GoToExportAndImport()
        {
            path = "/Views/ExportAndImport.xaml";
            assembly = Assembly.Occasional;
            Navigate();
        }

        public void GoToSettings()
        {
            path = "/Views/Settings.xaml";
            assembly = Assembly.Occasional;
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
                case Assembly.Occasional:
                    return "/Dietphone.Occasional;component";
                case Assembly.Medium:
                    return "/Dietphone.Medium;component";
                default:
                    return string.Empty;
            }
        }
    }
}