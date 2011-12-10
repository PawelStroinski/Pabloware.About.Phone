using Dietphone.Models;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Dietphone.Tools;

namespace Dietphone.ViewModels
{
    public class ProductEditingViewModel : EditingViewModelBase<Product>
    {
        public ObservableCollection<CategoryViewModel> Categories { get; private set; }
        public ProductViewModel Product { get; private set; }
        private List<CategoryViewModel> addedCategories = new List<CategoryViewModel>();
        private List<CategoryViewModel> deletedCategories = new List<CategoryViewModel>();

        public ProductEditingViewModel(Factories factories, Navigator navigator, StateProvider stateProvider)
            : base(factories, navigator, stateProvider)
        {
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

        public List<string> AllServingSizeUnits
        {
            get
            {
                return UnitAbbreviations.GetAbbreviationsFiltered(unit => unit != Unit.ServingSize);
            }
        }

        public void AddAndSetCategory(string name)
        {
            var tempModel = factories.CreateCategory();
            var models = factories.Categories;
            models.Remove(tempModel);
            var viewModel = new CategoryViewModel(tempModel, factories);
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
            Product.Category = Categories.GetNextItemToSelectWhenDeleteSelected(toDelete);
            Categories.Remove(toDelete);
            deletedCategories.Add(toDelete);
        }

        public void SaveAndReturn()
        {
            modelSource.CopyFrom(modelCopy);
            SaveCategories();
            navigator.GoBack();
        }

        public void DeleteAndSaveAndReturn()
        {
            var models = factories.Products;
            models.Remove(modelSource);
            SaveCategories();
            navigator.GoBack();
        }

        protected override void FindAndCopyModel()
        {
            var id = navigator.GetProductIdToEdit();
            modelSource = finder.FindProductById(id);
            if (modelSource != null)
            {
                modelCopy = modelSource.GetCopy();
                modelCopy.SetOwner(factories);
            }
        }

        protected override void UntombstoneModel()
        {
        }
        
        protected override void MakeViewModel()
        {
            LoadCategories();
            CreateProductViewModel();
        }

        protected override string Validate()
        {
            return modelCopy.Validate();
        }

        private void LoadCategories()
        {
            var loader = new ProductListingViewModel.CategoriesAndProductsLoader(factories);
            Categories = loader.Categories;
            foreach (var category in Categories)
            {
                category.MakeBuffer();
            }
        }

        private void CreateProductViewModel()
        {
            var maxCuAndFpu = new MaxCuAndFpuInCategories(finder, modelCopy);
            Product = new ProductViewModel(modelCopy)
            {
                Categories = Categories,
                MaxCuAndFpu = maxCuAndFpu
            };
            Product.PropertyChanged += delegate { OnGotDirty(); };
        }

        private void SaveCategories()
        {
            foreach (var category in Categories)
            {
                category.FlushBuffer();
            }
            var models = factories.Categories;
            foreach (var category in addedCategories)
            {
                models.Add(category.Model);
            }
            foreach (var category in deletedCategories)
            {
                models.Remove(category.Model);
            }
        }
    }
}
