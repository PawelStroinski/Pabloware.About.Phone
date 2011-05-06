using Dietphone.Models;
using Dietphone.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Telerik.Windows.Data;
using System.Threading;

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
        public event EventHandler BeforeRefresh;
        public event EventHandler AfterRefresh;
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

        public override void Refresh()
        {
            OnBeforeRefresh();
            maxNutritives.Reset();
            var loader = new CategoriesAndProductsLoader(this);
            loader.LoadAsync();
            loader.AfterLoad += delegate { OnAfterRefresh(); };
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

        public ProductViewModel FindProduct(Guid productId)
        {
            var result = from product in Products
                         where product.Id == productId
                         select product;
            return result.FirstOrDefault();
        }

        public CategoryViewModel FindCategory(Guid categoryId)
        {
            var result = from category in Categories
                         where category.Id == categoryId
                         select category;
            return result.FirstOrDefault();
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
                         where category.Id == product.Product.CategoryId
                         select category;
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

        private void OnSelectedProductChanged()
        {
            if (SelectedProduct != null)
            {
                IsBusy = true;
                Navigator.GoToProductEditing(SelectedProduct.Id);
            }
        }

        private void OnBeforeRefresh()
        {
            if (BeforeRefresh != null)
            {
                BeforeRefresh(this, EventArgs.Empty);
            }
        }

        private void OnAfterRefresh()
        {
            if (AfterRefresh != null)
            {
                AfterRefresh(this, EventArgs.Empty);
            }
        }

        public class CategoriesAndProductsLoader
        {
            public EventHandler AfterLoad;
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
                worker.DoWork += delegate { DoWork(); };
                worker.RunWorkerCompleted += delegate { WorkCompleted(); };
                viewModel.IsBusy = true;
                isLoading = true;
                worker.RunWorkerAsync();
            }

            public ObservableCollection<CategoryViewModel> GetCategoriesReloaded()
            {
                categories.Clear();
                LoadCategories();
                return categories;
            }

            private void DoWork()
            {
                LoadCategories();
                LoadProducts();
            }

            private void WorkCompleted()
            {
                AssignCategories();
                AssignProducts();
                viewModel.IsBusy = false;
                isLoading = false;
                OnAfterLoad();
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

            private void OnAfterLoad()
            {
                if (AfterLoad != null)
                {
                    AfterLoad(this, EventArgs.Empty);
                }
            }
        }
    }
}
