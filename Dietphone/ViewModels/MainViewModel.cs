using System;
using Dietphone.Models;

namespace Dietphone.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public ProductListingViewModel ProductListingViewModel { private get; set; }
        public MealItemEditingViewModel MealItemEditingViewModel { private get; set; }
        public event EventHandler ShowProductsOnly;
        public event EventHandler<AddingEnteredMealItemEventArgs> AddingEnteredMealItem;
        private string search = string.Empty;
        private Navigator navigator;
        private MealItemViewModel mealItem;
        private readonly Factories factories;
        private bool addEnteredMealItem;

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

        public void AddEnteredMealItem()
        {
            if (addEnteredMealItem)
            {
                var args = new AddingEnteredMealItemEventArgs()
                {
                    MealItem = mealItem
                };
                OnAddingEnteredMealItem(args);
            }
        }

        protected void OnNavigatorChanged()
        {
            Guid mealId = navigator.GetMealIdToAddMealItemTo();
            if (mealId != Guid.Empty)
            {
                var finder = factories.Finder;
                var meal = finder.FindMealById(mealId);
                if (meal == null)
                {
                    navigator.GoToMain();
                }
                else
                {
                    var mealViewModel = new MealViewModel(meal);
                    mealItem = mealViewModel.AddItem();
                    mealViewModel.DeleteItem(mealItem);
                    OnShowProductsOnly();
                    ProductListingViewModel.SelectedProductChanged += ProductListingViewModel_SelectedProductChanged;
                    MealItemEditingViewModel.MealItem = mealItem;
                    MealItemEditingViewModel.Confirming += delegate
                    {
                        addEnteredMealItem = true;
                        navigator.GoBack();
                    };
                }
            }
        }

        private void ProductListingViewModel_SelectedProductChanged(object sender, SelectedProductChangedEventArgs e)
        {
            e.Handled = true;
            mealItem.Value = string.Empty;
            mealItem.ProductId = ProductListingViewModel.SelectedProduct.Id;
            MealItemEditingViewModel.Show();
            ProductListingViewModel.SelectedProduct = null;
        }

        protected void OnShowProductsOnly()
        {
            if (ShowProductsOnly != null)
            {
                ShowProductsOnly(this, EventArgs.Empty);
            }
        }

        protected void OnAddingEnteredMealItem(AddingEnteredMealItemEventArgs args)
        {
            if (AddingEnteredMealItem != null)
            {
                AddingEnteredMealItem(this, args);
            }
        }
    }

    public class AddingEnteredMealItemEventArgs : EventArgs
    {
        public MealItemViewModel MealItem { get; set; }
    }
}