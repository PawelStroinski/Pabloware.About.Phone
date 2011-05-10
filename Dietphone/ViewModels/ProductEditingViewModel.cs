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
    public class ProductEditingViewModel : ViewModelBase
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

        public string CategoryName
        {
            get
            {
                var category = Product.Category;
                return category.Name;
            }
            set
            {
                var category = Product.Category;
                category.Name = value;
            }
        }

        public void AddAndSetCategory(string name)
        {
            var model = factories.CreateCategory();
            var viewModel = new CategoryViewModel(model);
            viewModel.Name = name;
            Categories.Add(viewModel);
            Product.Category = viewModel;
        }

        public bool CanDeleteCategory()
        {
            var categoryId = model.CategoryId;
            var productsInCategory = finder.FindProductsByCategory(categoryId);
            return productsInCategory.Count == 1 && Categories.Count > 1;
        }

        public void DeleteCategory()
        {
            var delete = Product.Category;
            var newIndex = Categories.IndexOf(delete) + 1;
            if (newIndex > Categories.Count - 1)
            {
                newIndex -= 2;
            }
            Product.Category = Categories[newIndex];
            Categories.Remove(delete);
            var models = factories.Categories;
            models.Remove(delete.Category);
        }

        private void FindModel()
        {
            var id = navigator.GetPassedProductId();
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
