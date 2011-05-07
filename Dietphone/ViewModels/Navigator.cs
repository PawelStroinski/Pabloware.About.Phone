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
using System.Windows.Navigation;
using System.Collections.Generic;

namespace Dietphone.ViewModels
{
    public interface Navigator
    {
        void GoToProductEditing(Guid productId);
        void GoToMain();
        Guid GetPassedProductId();
    }

    public class NavigatorImpl : Navigator
    {
        private readonly NavigationService service;
        private readonly IDictionary<string, string> passedQueryString;
        private const string PRODUCT_ID = "ProductId";

        public NavigatorImpl(NavigationService service, NavigationContext context)
        {
            this.service = service;
            passedQueryString = context.QueryString;
        }

        public void GoToProductEditing(Guid productId)
        {
            var destination = new UriBuilder();
            destination.Path = "/Views/ProductEditing.xaml";
            var id = productId.ToString();
            destination.Query = String.Format("{0}={1}", PRODUCT_ID, id);
            Navigate(destination);
        }

        public void GoToMain()
        {
            var destination = new UriBuilder();
            destination.Path = "/Views/Main.xaml";
            Navigate(destination);
        }

        public Guid GetPassedProductId()
        {
            if (passedQueryString.ContainsKey(PRODUCT_ID))
            {
                return new Guid(passedQueryString[PRODUCT_ID]);
            }
            else
            {
                return Guid.Empty;
            }
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