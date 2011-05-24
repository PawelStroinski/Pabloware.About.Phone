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
        public IEnumerable<MealNameViewModel> MealNames { private get; set; }
        public MealNameViewModel DefaultMealName { private get; set; }
        public DateViewModel Date { get; set; }
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

        public DateTime DateTime
        {
            get
            {
                var universal = Meal.DateTime;
                return universal.ToLocalTime();
            }
            set
            {
                var universal = value.ToUniversalTime();
                Meal.DateTime = universal;
                OnPropertyChanged("DateTime");
                OnPropertyChanged("DateOnly");
                OnPropertyChanged("DateAndTime");
                OnPropertyChanged("Time");
            }
        }

        public DateTime DateOnly
        {
            get
            {
                return DateTime.Date;
            }
        }

        public string DateAndTime
        {
            get
            {
                var date = DateTime.ToShortDateInAlternativeFormat();
                return string.Format("{0} {1}", date, Time);
            }
        }

        public string Time
        {
            get
            {
                return DateTime.ToShortTimeString();
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
                nameCache = FindName();
                isNameCached = true;
                return nameCache;
            }
            set
            {
                Meal.NameId = value.Id;
                isNameCached = false;
                OnPropertyChanged("Name");
                OnPropertyChanged("VisibleWhenIsNewerAndHasName");
                OnPropertyChanged("VisibleWhenIsNewerAndHasNoName");
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
                return string.Format("{0} KCAL", result);
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

        public Visibility VisibleWhenIsNewerAndHasName
        {
            get
            {
                return (IsNewer && HasName).ToVisibility();
            }
        }

        public Visibility VisibleWhenIsNewerAndHasNoName
        {
            get
            {
                return (IsNewer && !HasName).ToVisibility();
            }
        }

        public Visibility VisibleWhenIsOlder
        {
            get
            {
                return (!IsNewer).ToVisibility();
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
            if (name != DefaultMealName)
            {
                var nameOfName = name.Name;
                if (nameOfName.ContainsIgnoringCase(filter))
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
            if (Note.ContainsIgnoringCase(filter))
            {
                return true;
            }
            return false;
        }

        private bool IsNewer
        {
            get
            {
                if (Date == null)
                {
                    return true;
                }
                else
                {
                    return !Date.IsGroupOfOlder;
                }
            }
        }

        private bool HasName
        {
            get
            {
                return Name != DefaultMealName;
            }
        }

        private MealNameViewModel FindName()
        {
            if (Meal.NameId == Guid.Empty)
            {
                return DefaultMealName;
            }
            var result = from viewModel in MealNames
                         where viewModel.Id == Meal.NameId
                         select viewModel;
            result = result.DefaultIfEmpty(DefaultMealName);
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
            // Linq z Take() ewaluuje tylko tyle elementów listy ile potrzeba
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
