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
using Dietphone.Models;

namespace Dietphone.ViewModels
{
    public class ProductEditingViewModel
    {
        public ProductViewModel Product { get; private set; }
        private Finder finder;
        private Navigator navigator;
        private Product model;

        public ProductEditingViewModel(Factories factories, Navigator navigator)
        {
            this.finder = factories.Finder;
            this.navigator = navigator;
            FindModel();
            if (model == null)
            {
                navigator.GoToMain();
            }
            else
            {
                CreateViewModel();
            }
        }

        public void GoingToProductListing(ProductListingViewModel listingViewModel)
        {
            if (model != null)
            {
                listingViewModel.InvalidateProduct(model.Id);
            }
        }

        private void FindModel()
        {
            var id = navigator.GetProductId();
            model = finder.FindProductById(id);
        }

        private void CreateViewModel()
        {
            var maxNutritives = new MaxNutritivesInCategories(finder);
            Product = new ProductViewModel(model, maxNutritives);
        }
    }
}
