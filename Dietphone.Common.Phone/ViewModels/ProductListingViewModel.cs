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
        public event EventHandler<ChoosedEventArgs> Choosed;
        private readonly Factories factories;
        private readonly MaxCuAndFpuInCategories maxCuAndFpu;

        public ProductListingViewModel(Factories factories)
        {
            this.factories = factories;
            maxCuAndFpu = new MaxCuAndFpuInCategories(factories.Finder);
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
            if (Categories != null && Products != null)
            {
                maxCuAndFpu.Reset();
                var loader = new CategoriesAndProductsLoader(this);
                loader.LoadAsync();
                loader.Loaded += delegate { OnRefreshed(); };
            }
        }

        public void Choose(ProductViewModel product)
        {
            var e = new ChoosedEventArgs()
            {
                Product = product
            };
            OnChoosed(e);
            if (!e.Handled)
            {
                Navigator.GoToProductEditing(product.Id);
            }
        }

        public override void Add()
        {
            var product = factories.CreateProduct();
            product.AddedByUser = true;
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

        protected void OnChoosed(ChoosedEventArgs e)
        {
            if (Choosed != null)
            {
                Choosed(this, e);
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

            public ObservableCollection<CategoryViewModel> Categories
            {
                get
                {
                    if (categories == null)
                    {
                        LoadCategories();
                    }
                    return categories;
                }
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
                    var viewModel = new CategoryViewModel(model, factories);
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

    public class ChoosedEventArgs : EventArgs
    {
        public ProductViewModel Product { get; set; }
        public bool Handled { get; set; }
    }
}
