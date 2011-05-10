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
using System.ComponentModel;

namespace Dietphone.ViewModels
{
    public class ProductEditingViewModel : ViewModelBase
    {
        public ObservableCollection<CategoryViewModel> Categories { get; private set; }
        public ProductViewModel Product { get; private set; }
        public event EventHandler GotDirty;
        public event EventHandler<CannotSaveEventArgs> CannotSave;
        private Factories factories;
        private Finder finder;
        private Navigator navigator;
        private Product model;
        private Product modelSource;

        public ProductEditingViewModel(Factories factories, Navigator navigator)
        {
            this.factories = factories;
            this.finder = factories.Finder;
            this.navigator = navigator;
            FindAndCopyModel();
            if (model == null)
            {
                navigator.GoBack();
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

        public bool CanSave()
        {
            var validation = model.Validate();
            if (!string.IsNullOrEmpty(validation))
            {
                var args = new CannotSaveEventArgs();
                args.Reason = validation;
                OnCannotSave(args);
                return args.Ignore;
            }
            return true;
        }

        public void SaveAndReturn()
        {
            model.CopyToSameType(modelSource);
            navigator.GoBack();
        }

        public void CancelAndReturn()
        {
            navigator.GoBack();
        }

        private void FindAndCopyModel()
        {
            var id = navigator.GetPassedProductId();
            modelSource = finder.FindProductById(id);
            if (modelSource != null)
            {
                model = new Product();
                modelSource.CopyToSameType(model);
                model.Owner = factories;
            }
        }

        private void CreateProductViewModel()
        {
            var maxNutritives = new MaxNutritivesInCategories(finder);
            Product = new ProductViewModel(model, maxNutritives);
            Product.PropertyChanged += delegate { OnGotDirty(); };
        }

        private void LoadCategories()
        {
            var loader = new ProductListingViewModel.CategoriesAndProductsLoader(factories);
            Categories = loader.GetCategoriesReloaded();
            Product.Categories = Categories;
        }

        protected void OnGotDirty()
        {
            if (GotDirty != null)
            {
                GotDirty(this, EventArgs.Empty);
            }
        }

        protected void OnCannotSave(CannotSaveEventArgs e)
        {
            if (CannotSave != null)
            {
                CannotSave(this, e);
            }
        }
    }

    public class CannotSaveEventArgs : EventArgs
    {
        public string Reason { get; set; }
        public bool Ignore { get; set; }
    }
}
