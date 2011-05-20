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
        Guid GetPassedMealId();
        Guid GetPassedProductId();
    }

    public class NavigatorImpl : Navigator
    {
        private string path;
        private string idName;
        private Guid idValue;
        private readonly NavigationService service;
        private readonly IDictionary<string, string> passedQueryString;
        private const string MEAL_ID = "MealId";
        private const string PRODUCT_ID = "ProductId";

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
            idName = MEAL_ID;
            idValue = mealId;
            path = "/Views/MealEditing.xaml";
            NavigateWithId();
        }

        public void GoToProductEditing(Guid productId)
        {
            idName = PRODUCT_ID;
            idValue = productId;
            path = "/Views/ProductEditing.xaml";
            NavigateWithId();
        }

        public void GoToMain()
        {
            path = "/Views/Main.xaml";
            Navigate();
        }

        public Guid GetPassedMealId()
        {
            idName = MEAL_ID;
            return GetPassedId();
        }

        public Guid GetPassedProductId()
        {
            idName = PRODUCT_ID;
            return GetPassedId();
        }

        private Guid GetPassedId()
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

        private void Navigate(UriBuilder destination)
        {
            destination.Scheme = "";
            destination.Host = "";
            var uri = new Uri(destination.ToString(), UriKind.Relative);
            service.Navigate(uri);
        }
    }
}