using Dietphone.Models;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Dietphone.Tools;
using System;
using System.Globalization;
using System.ComponentModel;

namespace Dietphone.ViewModels
{
    public class MealEditingViewModel : EditingViewModelBase<Meal>
    {
        public ObservableCollection<MealNameViewModel> Names { get; private set; }
        public MealViewModel Meal { get; private set; }
        private List<MealNameViewModel> addedNames = new List<MealNameViewModel>();
        private List<MealNameViewModel> deletedNames = new List<MealNameViewModel>();
        private MealNameViewModel defaultName;
        private bool isLockedDateTime;
        private bool updatingLockedDateTime;
        private MealItemEditingViewModel itemEditing;
        private MealItemViewModel editItem;
        private const byte LOCKED_DATE_TIME_RECENT_MINUTES = 3;

        public MealEditingViewModel(Factories factories, Navigator navigator)
            : base(factories, navigator)
        {
            LockRecentDateTime();
        }

        public MealItemEditingViewModel ItemEditing
        {
            private get
            {
                return itemEditing;
            }
            set
            {
                if (itemEditing != value)
                {
                    itemEditing = value;
                    OnItemEditingChanged();
                }
            }
        }

        public string NameOfName
        {
            get
            {
                var name = Meal.Name;
                return name.Name;
            }
            set
            {
                var name = Meal.Name;
                name.Name = value;
            }
        }

        public string IdentifiableName
        {
            get
            {
                return string.Format("{0}, {1}", NameOfName, Meal.DateAndTime);
            }
        }

        public string DateFormat
        {
            get
            {
                var culture = CultureInfo.CurrentCulture;
                return culture.GetShortDateAlternativeFormat();
            }
        }

        // Uwaga: zmiana NotIsLockedDateTime może zmienić Meal.DateTime za pomocą UpdateLockedDateTime().
        public bool NotIsLockedDateTime
        {
            get
            {
                return !isLockedDateTime;
            }
            set
            {
                if (!isLockedDateTime != value)
                {
                    isLockedDateTime = !value;
                    UpdateLockedDateTime();
                    OnPropertyChanged("NotIsLockedDateTime");
                }
            }
        }

        public void AddAndSetName(string name)
        {
            var tempModel = factories.CreateMealName();
            var models = factories.MealNames;
            models.Remove(tempModel);
            var viewModel = new MealNameViewModel(tempModel, factories);
            viewModel.Name = name;
            Names.Add(viewModel);
            Meal.Name = viewModel;
            addedNames.Add(viewModel);
        }

        public bool CanEditName()
        {
            return Meal.Name != defaultName;
        }

        public bool CanDeleteName()
        {
            return Meal.Name != defaultName;
        }

        public void DeleteName()
        {
            var toDelete = Meal.Name;
            Meal.Name = Names.GetNextItemToSelectWhenDeleteSelected(toDelete);
            Names.Remove(toDelete);
            deletedNames.Add(toDelete);
        }

        public void UpdateTimeAndSaveAndReturn()
        {
            UpdateLockedDateTime();
            modelSource.CopyFrom(modelCopy);
            modelSource.CopyItemsFrom(modelCopy);
            SaveNames();
            navigator.GoBack();
        }

        public void DeleteAndSaveAndReturn()
        {
            var models = factories.Meals;
            models.Remove(modelSource);
            SaveNames();
            navigator.GoBack();
        }

        public void AddItem()
        {
            navigator.GoToMainToAddMealItem();
        }

        public void AddCopyOfItem(MealItem source)
        {
            var item = Meal.AddItem();
            item.CopyFromModel(source);
        }

        public void EditItem(MealItemViewModel itemViewModel)
        {
            if (itemViewModel != null)
            {
                editItem = itemViewModel;
                editItem.MakeBuffer();
                ItemEditing.Show(editItem);
            }
        }

        protected override void FindAndCopyModel()
        {
            var id = navigator.GetMealIdToEdit();
            modelSource = finder.FindMealById(id);
            if (modelSource != null)
            {
                modelCopy = modelSource.GetCopy();
                modelCopy.SetOwner(factories);
                modelCopy.CopyItemsFrom(modelSource);
            }
        }

        protected override void MakeViewModel()
        {
            LoadNames();
            CreateMealViewModel();
        }

        protected override string Validate()
        {
            return modelCopy.Validate();
        }

        protected void OnItemEditingChanged()
        {
            ItemEditing.Confirmed += delegate
            {
                editItem.FlushBuffer();
                editItem.Invalidate();
            };
            ItemEditing.Cancelled += delegate
            {
                editItem.ClearBuffer();
                editItem.Invalidate();
            };
            ItemEditing.NeedToDelete += delegate
            {
                Meal.DeleteItem(editItem);
            };
            ItemEditing.CanDelete = true;
        }

        private void LoadNames()
        {
            var loader = new MealListingViewModel.NamesAndMealsLoader(factories, true);
            Names = loader.Names;
            foreach (var mealName in Names)
            {
                mealName.MakeBuffer();
            }
            defaultName = loader.DefaultName;
            Names.Insert(0, defaultName);
        }

        private void CreateMealViewModel()
        {
            Meal = new MealViewModel(modelCopy, factories)
            {
                Names = Names,
                DefaultName = defaultName
            };
            Meal.PropertyChanged += Meal_PropertyChanged;
        }

        private void Meal_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnGotDirty();
            if (e.PropertyName == "DateTime" && !updatingLockedDateTime)
            {
                NotIsLockedDateTime = true;
            }
        }

        private void SaveNames()
        {
            foreach (var viewModel in Names)
            {
                viewModel.FlushBuffer();
            }
            var models = factories.MealNames;
            foreach (var viewModel in addedNames)
            {
                models.Add(viewModel.Model);
            }
            foreach (var viewModel in deletedNames)
            {
                models.Remove(viewModel.Model);
            }
        }

        private void LockRecentDateTime()
        {
            var difference = (DateTime.Now - Meal.DateTime).Duration();
            isLockedDateTime = difference <= TimeSpan.FromMinutes(LOCKED_DATE_TIME_RECENT_MINUTES);
        }

        private void UpdateLockedDateTime()
        {
            if (isLockedDateTime)
            {
                updatingLockedDateTime = true;
                Meal.DateTime = DateTime.Now;
                updatingLockedDateTime = false;
            }
        }
    }
}
