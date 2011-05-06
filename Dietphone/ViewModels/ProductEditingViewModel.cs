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
using System.Collections.ObjectModel;

namespace Dietphone.ViewModels
{
    public class ProductEditingViewModel
    {
        public ObservableCollection<CategoryViewModel> Categories { get; private set; }
        public ProductViewModel Product { get; private set; }
        private Factories factories;
        private Finder finder;
        private Navigator navigator;
        private Product model;

        public ProductEditingViewModel(Factories factories, Navigator navigator)
        {
            this.factories = factories;
            this.finder = factories.Finder;
            this.navigator = navigator;
            FindModel();
            if (model == null)
            {
                navigator.GoToMain();
            }
            else
            {
                CreateProductViewModel();
                LoadCategories();
            }
        }

        private void FindModel()
        {
            var id = navigator.GetProductId();
            model = finder.FindProductById(id);
        }

        private void CreateProductViewModel()
        {
            var maxNutritives = new MaxNutritivesInCategories(finder);
            Product = new ProductViewModel(model, maxNutritives);
        }

        private void LoadCategories()
        {
            var loader = new ProductListingViewModel.CategoriesAndProductsLoader(factories);
            Categories = loader.GetCategoriesReloaded();
            Product.Categories = Categories;
        }
    }
}
