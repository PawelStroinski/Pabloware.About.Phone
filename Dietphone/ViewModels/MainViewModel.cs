using System;
using Dietphone.Models;

namespace Dietphone.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public ProductListingViewModel ProductListing { private get; set; }
        public MealItemEditingViewModel MealItemEditing { private get; set; }
        public MealEditingViewModel MealEditing { private get; set; }
        public ExportAndImportViewModel ExportAndImport { private get; set; }
        public event EventHandler ShowProductsOnly;
        private string search = string.Empty;
        private Navigator navigator;
        private MealItem tempMealItem;
        private bool addMealItem;
        private readonly Factories factories;

        public MainViewModel(Factories factories)
        {
            this.factories = factories;
        }

        public string Search
        {
            get
            {
                return search;
            }
            set
            {
                if (search != value)
                {
                    search = value;
                    OnPropertyChanged("Search");
                }
            }
        }

        public Navigator Navigator
        {
            set
            {
                navigator = value;
                OnNavigatorChanged();
            }
        }

        public void ReturningToMealEditing()
        {
            if (addMealItem)
            {
                MealEditing.AddCopyOfItem(tempMealItem);
            }
        }

        public void GoingToExportAndImport()
        {
            Models.ExportAndImport implementation = new ExportAndImportImpl(factories);
            ExportAndImport.Implementation = implementation;
        }

        public void About()
        {
            navigator.GoToAbout();
        }

        protected void OnNavigatorChanged()
        {
            if (navigator.ShouldAddMealItem())
            {
                AddMealItem();
            }
        }

        private void AddMealItem()
        {
            ProductListing.Choosed -= ProductListing_Choosed;
            ProductListing.Choosed += ProductListing_Choosed;
            MealItemEditing.Confirmed -= MealItemEditing_Confirmed;
            MealItemEditing.Confirmed += MealItemEditing_Confirmed;
            OnShowProductsOnly();
        }

        private void ProductListing_Choosed(object sender, ChoosedEventArgs e)
        {
            AddMealItemWithProduct(e.Product);
            e.Handled = true;
        }

        private void MealItemEditing_Confirmed(object sender, EventArgs e)
        {
            addMealItem = true;
            navigator.GoBack();
        }

        private void AddMealItemWithProduct(ProductViewModel product)
        {
            tempMealItem = factories.CreateMealItem();
            tempMealItem.ProductId = product.Id;
            var tempViewModel = new MealItemViewModel(tempMealItem, factories);
            MealItemEditing.Show(tempViewModel);
        }

        protected void OnShowProductsOnly()
        {
            if (ShowProductsOnly != null)
            {
                ShowProductsOnly(this, EventArgs.Empty);
            }
        }
    }
}