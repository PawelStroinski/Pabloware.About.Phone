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
using System.Collections.ObjectModel;
using Telerik.Windows.Data;
using Dietphone.Models;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace Dietphone.ViewModels
{
    public class ProductListingViewModel : SubViewModel
    {
        public ObservableCollection<ProductViewModel> Products { get; private set; }
        public ObservableCollection<CategoryViewModel> Categories { get; private set; }
        public ObservableCollection<DataDescriptor> GroupDescriptors { private get; set; }
        public ObservableCollection<DataDescriptor> SortDescriptors { private get; set; }
        public ObservableCollection<DataDescriptor> FilterDescriptors { private get; set; }
        public event EventHandler BeginDataUpdate;
        public event EventHandler EndDataUpdate;
        public event EventHandler Invalidate;
        private Factories factories;
        private MaxNutritivesInCategories maxNutritives;
        private ProductViewModel selectedProduct;

        public ProductListingViewModel(Factories factories)
        {
            this.factories = factories;
            maxNutritives = new MaxNutritivesInCategories(factories.Finder);
        }

        public ProductViewModel SelectedProduct
        {
            get
            {
                return selectedProduct;
            }
            set
            {
                if (selectedProduct != value)
                {
                    selectedProduct = value;
                    OnSelectedProductChanged();
                }
            }
        }

        public override void Load()
        {
            if (Categories == null && Products == null)
            {
                var loader = new CategoriesAndProductsLoader(this);
                loader.LoadAsync();
            }
        }

        public void UpdateGroupDescriptors()
        {
            GroupDescriptors.Clear();
            var groupByCategory = new GenericGroupDescriptor<ProductViewModel, CategoryViewModel>(FindCategoryFromProduct);
            GroupDescriptors.Add(groupByCategory);
        }

        public void UpdateSortDescriptors()
        {
            SortDescriptors.Clear();
            var sortByName = new GenericSortDescriptor<ProductViewModel, string>(product => product.Name);
            SortDescriptors.Add(sortByName);
        }

        public void InvalidateProduct(Guid productId)
        {
            if (Products != null)
            {
                var product = FindProduct(productId);
                if (product != null)
                {
                    product.Invalidate();
                    OnInvalidate();
                }
            }
        }

        protected override void OnSearchChanged()
        {
            OnBeginDataUpdate();
            UpdateFilterDescriptors();
            OnEndDataUpdate();
        }

        private void UpdateFilterDescriptors()
        {
            FilterDescriptors.Clear();
            if (search != "")
            {
                var filterByName = new GenericFilterDescriptor<ProductViewModel>(product => product.Name.ContainsIgnoringCase(search));
                FilterDescriptors.Add(filterByName);
            }
        }

        private CategoryViewModel FindCategoryFromProduct(ProductViewModel product)
        {
            if (product == null)
            {
                throw new NullReferenceException("product");
            }
            var result = from category in Categories
                         where category.Category.Id == product.Product.CategoryId
                         select category;
            return result.FirstOrDefault();
        }

        private ProductViewModel FindProduct(Guid productId)
        {
            var result = from product in Products
                         where product.Product.Id == productId
                         select product;
            return result.FirstOrDefault();
        }

        private void OnBeginDataUpdate()
        {
            if (BeginDataUpdate != null)
            {
                BeginDataUpdate(this, EventArgs.Empty);
            }
        }

        private void OnEndDataUpdate()
        {
            if (EndDataUpdate != null)
            {
                EndDataUpdate(this, EventArgs.Empty);
            }
        }

        private void OnInvalidate()
        {
            if (Invalidate != null)
            {
                Invalidate(this, EventArgs.Empty);
            }
        }

        private void OnSelectedProductChanged()
        {
            if (SelectedProduct != null)
            {
                var model = SelectedProduct.Product;
                Navigator.GoToProductEditing(model.Id);
                SelectedProduct = null;
            }
            OnPropertyChanged("SelectedProduct");
        }

        public class CategoriesAndProductsLoader
        {
            private ObservableCollection<CategoryViewModel> categories = new ObservableCollection<CategoryViewModel>();
            private ObservableCollection<ProductViewModel> products = new ObservableCollection<ProductViewModel>();
            private ProductListingViewModel viewModel;
            private Factories factories;
            private MaxNutritivesInCategories maxNutritives;
            private bool isLoading;

            public CategoriesAndProductsLoader(ProductListingViewModel viewModel)
            {
                this.viewModel = viewModel;
                factories = viewModel.factories;
                maxNutritives = viewModel.maxNutritives;
            }

            public CategoriesAndProductsLoader(Factories factories)
            {
                this.factories = factories;
            }

            public void LoadAsync()
            {
                if (viewModel == null)
                {
                    throw new InvalidOperationException("Use other constructor for this operation.");
                }
                if (isLoading)
                {
                    return;
                }
                var worker = new BackgroundWorker();
                worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
                viewModel.IsBusy = true;
                isLoading = true;
                worker.RunWorkerAsync();
            }

            public ObservableCollection<CategoryViewModel> GetCategoriesSyncReloaded()
            {
                categories.Clear();
                LoadCategories();
                return categories;
            }

            private void Worker_DoWork(object sender, DoWorkEventArgs e)
            {
                LoadCategories();
                LoadProducts();
            }

            private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
            {
                AssignCategories();
                AssignProducts();
                viewModel.IsBusy = false;
                isLoading = false;
            }

            private void LoadCategories()
            {
                var models = factories.Categories;
                var unsortedViewModels = new List<CategoryViewModel>();
                foreach (var model in models)
                {
                    var viewModel = new CategoryViewModel(model);
                    unsortedViewModels.Add(viewModel);
                }
                var sortedViewModels = unsortedViewModels.OrderBy(category => category.Name);
                foreach (var viewModel in sortedViewModels)
                {
                    categories.Add(viewModel);
                }
            }

            private void LoadProducts()
            {
                var models = factories.Products;
                foreach (var model in models)
                {
                    var viewModel = new ProductViewModel(model, maxNutritives);
                    products.Add(viewModel);
                }
            }

            private void AssignCategories()
            {
                viewModel.Categories = categories;
                viewModel.OnPropertyChanged("Categories");
            }

            private void AssignProducts()
            {
                viewModel.Products = products;
                viewModel.OnPropertyChanged("Products");
            }
        }
    }
}
