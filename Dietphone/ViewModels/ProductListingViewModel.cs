using Dietphone.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Telerik.Windows.Data;
using Dietphone.Tools;

namespace Dietphone.ViewModels
{
    public class ProductListingViewModel : SubViewModel
    {
        public ObservableCollection<ProductViewModel> Products { get; private set; }
        public ObservableCollection<CategoryViewModel> Categories { get; private set; }
        public ObservableCollection<DataDescriptor> GroupDescriptors { private get; set; }
        public ObservableCollection<DataDescriptor> SortDescriptors { private get; set; }
        public ObservableCollection<DataDescriptor> FilterDescriptors { private get; set; }
        public event EventHandler DescriptorsUpdating;
        public event EventHandler DescriptorsUpdated;
        private Factories factories;
        private MaxCuAndFpuInCategories maxCuAndFpu;
        private ProductViewModel selectedProduct;

        public ProductListingViewModel(Factories factories)
        {
            this.factories = factories;
            maxCuAndFpu = new MaxCuAndFpuInCategories(factories.Finder);
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
                loader.Loaded += delegate { OnLoaded(); };
            }
        }

        public override void Refresh()
        {
            OnRefreshing();
            maxCuAndFpu.Reset();
            var loader = new CategoriesAndProductsLoader(this);
            loader.LoadAsync();
            loader.Loaded += delegate { OnRefreshed(); };
        }

        public override void Add()
        {
            var product = factories.CreateProduct();
            Navigator.GoToProductEditing(product.Id);
        }

        public void UpdateGroupDescriptors()
        {
            GroupDescriptors.Clear();
            var groupByCategory = new GenericGroupDescriptor<ProductViewModel, CategoryViewModel>(product => product.Category);
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
            OnDescriptorsUpdating();
            UpdateFilterDescriptors();
            OnDescriptorsUpdated();
        }

        private void UpdateFilterDescriptors()
        {
            FilterDescriptors.Clear();
            if (!string.IsNullOrEmpty(search))
            {
                var filterByName = new GenericFilterDescriptor<ProductViewModel>(product => product.Name.ContainsIgnoringCase(search));
                FilterDescriptors.Add(filterByName);
            }
        }

        protected void OnSelectedProductChanged()
        {
            if (SelectedProduct != null)
            {
                Navigator.GoToProductEditing(SelectedProduct.Id);
            }
        }

        protected void OnDescriptorsUpdating()
        {
            if (DescriptorsUpdating != null)
            {
                DescriptorsUpdating(this, EventArgs.Empty);
            }
        }

        protected void OnDescriptorsUpdated()
        {
            if (DescriptorsUpdated != null)
            {
                DescriptorsUpdated(this, EventArgs.Empty);
            }
        }

        public class CategoriesAndProductsLoader : LoaderBase
        {
            private ObservableCollection<CategoryViewModel> categories;
            private ObservableCollection<ProductViewModel> products;
            private MaxCuAndFpuInCategories maxCuAndFpu;

            public CategoriesAndProductsLoader(ProductListingViewModel viewModel)
            {
                this.viewModel = viewModel;
                factories = viewModel.factories;
                maxCuAndFpu = viewModel.maxCuAndFpu;
            }

            public CategoriesAndProductsLoader(Factories factories)
            {
                this.factories = factories;
            }

            public ObservableCollection<CategoryViewModel> GetCategoriesReloaded()
            {
                LoadCategories();
                return categories;
            }

            protected override void DoWork()
            {
                LoadCategories();
                LoadProducts();
            }

            protected override void WorkCompleted()
            {
                AssignCategories();
                AssignProducts();
                base.WorkCompleted();
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
                categories = new ObservableCollection<CategoryViewModel>();
                foreach (var viewModel in sortedViewModels)
                {
                    categories.Add(viewModel);
                }
            }

            private void LoadProducts()
            {
                var models = factories.Products;
                products = new ObservableCollection<ProductViewModel>();
                foreach (var model in models)
                {
                    var viewModel = new ProductViewModel(model)
                    {
                        Categories = categories,
                        MaxCuAndFpu = maxCuAndFpu
                    };
                    products.Add(viewModel);
                }
            }

            private void AssignCategories()
            {
                GetViewModel().Categories = categories;
                GetViewModel().OnPropertyChanged("Categories");
            }

            private void AssignProducts()
            {
                GetViewModel().Products = products;
                GetViewModel().OnPropertyChanged("Products");
            }

            private ProductListingViewModel GetViewModel()
            {
                return viewModel as ProductListingViewModel;
            }
        }
    }
}
