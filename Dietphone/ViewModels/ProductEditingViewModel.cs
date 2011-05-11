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
using System.Collections.Generic;

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
        private Product modelCopy;
        private Product modelSource;
        private List<CategoryViewModel> addedCategories = new List<CategoryViewModel>();
        private List<CategoryViewModel> deletedCategories = new List<CategoryViewModel>();

        public ProductEditingViewModel(Factories factories, Navigator navigator)
        {
            this.factories = factories;
            this.finder = factories.Finder;
            this.navigator = navigator;
            FindAndCopyModel();
            if (modelCopy == null)
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
            var tempModel = factories.CreateCategory();
            var models = factories.Categories;
            models.Remove(tempModel);
            var viewModel = new CategoryViewModel(tempModel);
            viewModel.Name = name;
            Categories.Add(viewModel);
            Product.Category = viewModel;
            addedCategories.Add(viewModel);
        }

        public bool CanDeleteCategory()
        {
            var categoryId = modelCopy.CategoryId;
            var productsInCategory = finder.FindProductsByCategory(categoryId);
            bool otherProductsInCategory;
            if (productsInCategory.Count == 0)
            {
                otherProductsInCategory = false;
            }
            else
                if (productsInCategory.Count == 1)
                {
                    otherProductsInCategory = productsInCategory[0] != modelSource;
                }
                else
                {
                    otherProductsInCategory = true;
                }
            return !otherProductsInCategory && Categories.Count > 1;
        }

        public void DeleteCategory()
        {
            var toDelete = Product.Category;
            var newIndex = Categories.IndexOf(toDelete) + 1;
            if (newIndex > Categories.Count - 1)
            {
                newIndex -= 2;
            }
            Product.Category = Categories[newIndex];
            Categories.Remove(toDelete);
            deletedCategories.Add(toDelete);
        }

        public bool CanSave()
        {
            var validation = modelCopy.Validate();
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
            modelCopy.CopyToSameType(modelSource);
            SaveAddingAndDeletingCategories();
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
                modelCopy = new Product();
                modelSource.CopyToSameType(modelCopy);
                modelCopy.Owner = factories;
            }
        }

        private void CreateProductViewModel()
        {
            var maxNutritives = new MaxNutritivesInCategories(finder);
            Product = new ProductViewModel(modelCopy, maxNutritives);
            Product.PropertyChanged += delegate { OnGotDirty(); };
        }

        private void LoadCategories()
        {
            var loader = new ProductListingViewModel.CategoriesAndProductsLoader(factories);
            Categories = loader.GetCategoriesReloaded();
            Product.Categories = Categories;
        }

        private void SaveAddingAndDeletingCategories()
        {
            var models = factories.Categories;
            foreach (var viewModel in addedCategories)
            {
                models.Add(viewModel.Category);
            }
            foreach (var viewModel in deletedCategories)
            {
                models.Remove(viewModel.Category);
            }
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
