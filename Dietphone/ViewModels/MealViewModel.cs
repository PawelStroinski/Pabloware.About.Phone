using System;
using Dietphone.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;
using Dietphone.Tools;
using System.Collections.Generic;

namespace Dietphone.ViewModels
{
    public class MealViewModel : ViewModelBase
    {
        public Meal Meal { get; private set; }
        public Collection<MealNameViewModel> MealNames { private get; set; }
        private ObservableCollection<MealItemViewModel> items;
        private bool isNameCached;
        private bool isProductsHeadCached;
        private bool isProductsTailCached;
        private MealNameViewModel nameCache;
        private string productsHeadCache;
        private IEnumerable<string> productsTailCache;
        private readonly object itemsLock = new object();
        private const byte TAKE_PRODUCTS_TO_HEAD = 3;

        public MealViewModel(Meal meal)
        {
            Meal = meal;
        }

        public Guid Id
        {
            get
            {
                return Meal.Id;
            }
        }

        public DateTime Date
        {
            get
            {
                return Meal.Date;
            }
            set
            {
                Meal.Date = value;
                OnPropertyChanged("Date");
                OnPropertyChanged("DateText");
            }
        }

        public DateTime DateOnly
        {
            get
            {
                return Date.Date;
            }
        }

        public string Time
        {
            get
            {
                return Date.ToShortTimeString();
            }
        }

        public MealNameViewModel Name
        {
            get
            {
                if (isNameCached)
                {
                    return nameCache;
                }
                if (MealNames == null)
                {
                    throw new InvalidOperationException("Set MealNames first.");
                }
                nameCache = FindName();
                isNameCached = true;
                return nameCache;
            }
            set
            {
                if (value == null)
                {
                    Meal.NameId = Guid.Empty;
                }
                else
                {
                    Meal.NameId = value.Id;
                }
                isNameCached = false;
                OnPropertyChanged("Name");
            }
        }

        public string ProductsHead
        {
            get
            {
                if (isProductsHeadCached)
                {
                    return productsHeadCache;
                }
                productsHeadCache = MakeProductsHead();
                isProductsHeadCached = true;
                return productsHeadCache;
            }
        }

        public IEnumerable<string> ProductsTail
        {
            get
            {
                if (isProductsTailCached)
                {
                    return productsTailCache;
                }
                productsTailCache = MakeProductsTail();
                isProductsTailCached = true;
                return productsTailCache;
            }
        }

        public string Note
        {
            get
            {
                return Meal.Note;
            }
            set
            {
                if (value != Meal.Note)
                {
                    Meal.Note = value;
                    OnPropertyChanged("Note");
                }
            }
        }

        public ObservableCollection<MealItemViewModel> Items
        {
            get
            {
                lock (itemsLock)
                {
                    if (items == null)
                    {
                        items = new ObservableCollection<MealItemViewModel>();
                        LoadItems();
                        items.CollectionChanged += delegate { OnItemsChanged(); };
                    }
                    return items;
                }
            }
        }

        public string Energy
        {
            get
            {
                var result = Meal.Energy;
                return string.Format("{0} kcal", result);
            }
        }

        public string Cu
        {
            get
            {
                var result = Meal.Cu;
                return string.Format("{0} WW", result);
            }
        }

        public string Fpu
        {
            get
            {
                var result = Meal.Fpu;
                return string.Format("{0} WBT", result);
            }
        }

        public Visibility VisibleWhenHasName
        {
            get
            {
                if (Name == null)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }

        public Visibility VisibleWhenHasNoName
        {
            get
            {
                if (Name == null)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        public MealItemViewModel AddItem()
        {
            var itemModel = Meal.AddItem();
            var itemViewModel = CreateItemViewModel(itemModel);
            Items.Add(itemViewModel);
            return itemViewModel;
        }

        public void DeleteItem(MealItemViewModel itemViewModel)
        {
            Meal.DeleteItem(itemViewModel.MealItem);
            Items.Remove(itemViewModel);
        }

        public bool FilterIn(string filter)
        {
            var name = Name;
            if (name != null)
            {
                var nameValue = name.Name;
                if (nameValue.ContainsIgnoringCase(filter))
                {
                    return true;
                }
            }
            if (ProductsHead.ContainsIgnoringCase(filter))
            {
                return true;
            }
            var tail = ProductsTail;
            foreach (var product in tail)
            {
                if (product.ContainsIgnoringCase(filter))
                {
                    return true;
                }
            }
            return false;
        }

        private MealNameViewModel FindName()
        {
            if (Meal.NameId == Guid.Empty)
            {
                return null;
            }
            var result = from viewModel in MealNames
                         where viewModel.Id == Meal.NameId
                         select viewModel;
            return result.FirstOrDefault();
        }

        private void LoadItems()
        {
            foreach (var itemModel in Meal.Items)
            {
                var itemViewModel = CreateItemViewModel(itemModel);
                items.Add(itemViewModel);
            }
        }

        private MealItemViewModel CreateItemViewModel(MealItem itemModel)
        {
            var itemViewModel = new MealItemViewModel(itemModel);
            itemViewModel.ItemChanged += delegate { OnItemsChanged(); };
            return itemViewModel;
        }

        private string MakeProductsHead()
        {
            var all = from item in Items
                      select item.ProductName;
            var nonEmpty = all.Where(name => !string.IsNullOrEmpty(name));
            var firstFew = nonEmpty.Take(TAKE_PRODUCTS_TO_HEAD);
            // Linq z Take() ewaluuje tylko tyle elementów listy ile trzeba
            return string.Join(" | ", firstFew.ToArray());
        }

        private IEnumerable<string> MakeProductsTail()
        {
            var result = new List<string>();
            var items = Items;
            // Nie zamieniaj na Linq bo Skip() bo w przeciwieństwie do Take() ewaluuje całą listę
            for (int i = TAKE_PRODUCTS_TO_HEAD; i < items.Count; i++)
            {
                var name = items[i].ProductName;
                result.Add(name);
            }
            return result;
        }

        protected void OnItemsChanged()
        {
            isProductsHeadCached = false;
            isProductsTailCached = false;
            OnPropertyChanged("Energy");
            OnPropertyChanged("Cu");
            OnPropertyChanged("Fpu");
        }
    }
}
