using System;
using System.Linq;
using Dietphone.Models;
using System.Collections.ObjectModel;

namespace Dietphone.ViewModels
{
    public class MealViewModel : ViewModelBase
    {
        public Meal Meal { get; private set; }
        public Collection<MealNameViewModel> MealNames { private get; set; }
        private ObservableCollection<MealItemViewModel> items;
        private readonly object itemsLock = new object();

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

        public string DateText
        {
            get
            {
                return string.Format("{0} {1}", Date.ToShortDateString(), Date.ToShortTimeString());
            }
        }

        public DateTime DateOnly
        {
            get
            {
                return Date.Date;
            }
        }

        public MealNameViewModel Name
        {
            get
            {
                if (MealNames == null)
                {
                    throw new InvalidOperationException("Set MealNames first.");
                }
                return FindName();
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
                OnPropertyChanged("Name");
            }
        }

        public string NameText
        {
            get
            {
                var name = Name;
                if (name == null)
                {
                    return string.Empty;
                }
                else
                {
                    return name.Name;
                }
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
                        items.CollectionChanged += delegate { OnNutrientsChanged(); };
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

        private MealNameViewModel FindName()
        {
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
            itemViewModel.NutrientsChanged += delegate { OnNutrientsChanged(); };
            return itemViewModel;
        }

        protected void OnNutrientsChanged()
        {
            OnPropertyChanged("Energy");
            OnPropertyChanged("Cu");
            OnPropertyChanged("Fpu");
        }
    }
}
