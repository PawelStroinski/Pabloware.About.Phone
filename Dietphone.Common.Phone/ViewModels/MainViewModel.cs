using System;
using Dietphone.Models;
using System.Collections.Generic;
using Dietphone.Tools;

namespace Dietphone.ViewModels
{
    public class MainViewModel : PivotTombstoningViewModel
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
        private const string MEAL_ITEM_EDITING = "MEAL_ITEM_EDITING";
        private const string MEAL_ITEM_PRODUCT = "MEAL_ITEM_PRODUCT";

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

        public void GoingToMealEditing()
        {
            if (addMealItem)
            {
                MealEditing.AddCopyOfThisItem = tempMealItem;
            }
        }

        public void About()
        {
            navigator.GoToAbout();
        }

        public void ExportAndImport()
        {
            navigator.GoToExportAndImport();
        }

        public void Settings()
        {
            navigator.GoToSettings();
        }

        public override void Tombstone()
        {
            base.Tombstone();
            TombstoneMealItemEditing();
        }

        public void UiRendered()
        {
            UntombstoneMealItemEditing();
        }

        protected void OnNavigatorChanged()
        {
            if (navigator.ShouldAddMealItem())
            {
                AddingMealItem();
            }
        }

        private void AddingMealItem()
        {
            ProductListing.Choosed -= ProductListing_Choosed;
            ProductListing.Choosed += ProductListing_Choosed;
            MealItemEditing.Confirmed -= MealItemEditing_Confirmed;
            MealItemEditing.Confirmed += MealItemEditing_Confirmed;
            MealItemEditing.StateProvider = StateProvider;
            OnShowProductsOnly();
        }

        private void TombstoneMealItemEditing()
        {
            var state = StateProvider.State;
            state[MEAL_ITEM_EDITING] = MealItemEditing.IsVisible;
            if (MealItemEditing.IsVisible)
            {
                var mealItem = MealItemEditing.MealItem;
                state[MEAL_ITEM_PRODUCT] = mealItem.ProductId;
                MealItemEditing.Tombstone();
            }
        }

        private void UntombstoneMealItemEditing()
        {
            var state = StateProvider.State;
            var mealItemEditing = false;
            if (state.ContainsKey(MEAL_ITEM_EDITING))
            {
                mealItemEditing = (bool)state[MEAL_ITEM_EDITING];
            }
            if (mealItemEditing)
            {
                var productId = (Guid)state[MEAL_ITEM_PRODUCT];
                var products = ProductListing.Products;
                var product = products.FindById(productId);
                if (product != null)
                {
                    AddMealItemWithProduct(product);
                }
            }
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