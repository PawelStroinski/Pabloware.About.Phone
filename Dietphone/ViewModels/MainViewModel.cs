using System;
using Dietphone.Models;

namespace Dietphone.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public ProductListingViewModel ProductListing { private get; set; }
        public MealItemEditingViewModel MealItemEditing { private get; set; }
        public MealEditingViewModel MealEditing { private get; set; }
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

        protected void OnNavigatorChanged()
        {
            if (navigator.ShouldAddMealItem())
            {
                AddMealItem();
            }
        }

        private void AddMealItem()
        {
            ProductListing.SelectedProductChanged += ProductListing_SelectedProductChanged;
            MealItemEditing.Confirmed += delegate
            {
                addMealItem = true;
                navigator.GoBack();
            };
            OnShowProductsOnly();
        }

        private void ProductListing_SelectedProductChanged(object sender, SelectedProductChangedEventArgs e)
        {
            AddMealItemWithProduct(ProductListing.SelectedProduct);
            e.Handled = true;
            ProductListing.SelectedProduct = null;
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