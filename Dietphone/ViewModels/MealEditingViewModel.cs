using Dietphone.Models;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Dietphone.Tools;
using System;
using System.Globalization;

namespace Dietphone.ViewModels
{
    public class MealEditingViewModel : EditingViewModelBase<Meal>
    {
        public ObservableCollection<MealNameViewModel> MealNames { get; private set; }
        public MealViewModel Meal { get; private set; }
        private List<MealNameViewModel> addedMealNames = new List<MealNameViewModel>();
        private List<MealNameViewModel> deletedMealNames = new List<MealNameViewModel>();
        private MealNameViewModel defaultMealName;

        public MealEditingViewModel(Factories factories, Navigator navigator)
            : base(factories, navigator)
        {
        }

        public string NameOfMealName
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
                return string.Format("{0}, {1}", NameOfMealName, Meal.DateAndTime);
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

        public void AddAndSetMealName(string name)
        {
            var tempModel = factories.CreateMealName();
            var models = factories.MealNames;
            models.Remove(tempModel);
            var viewModel = new MealNameViewModel(tempModel);
            viewModel.Name = name;
            MealNames.Add(viewModel);
            Meal.Name = viewModel;
            addedMealNames.Add(viewModel);
        }

        public bool CanRenameMealName()
        {
            return Meal.Name != defaultMealName;
        }

        public bool CanDeleteMealName()
        {
            return Meal.Name != defaultMealName;
        }

        public void DeleteMealName()
        {
            var toDelete = Meal.Name;
            Meal.Name = MealNames.GetNextItemToSelectWhenDeleteSelected(toDelete);
            MealNames.Remove(toDelete);
            deletedMealNames.Add(toDelete);
        }

        public void SaveAndReturn()
        {
            modelSource.CopyFrom(modelCopy);
            modelSource.CopyItemsFrom(modelCopy);
            SaveMealNames();
            navigator.GoBack();
        }

        public void DeleteAndSaveAndReturn()
        {
            var models = factories.Meals;
            models.Remove(modelSource);
            SaveMealNames();
            navigator.GoBack();
        }

        protected override void FindAndCopyModel()
        {
            var id = navigator.GetPassedMealId();
            modelSource = finder.FindMealById(id);
            if (modelSource != null)
            {
                modelCopy = modelSource.GetCopy();
                modelCopy.Owner = factories;
                modelCopy.CopyItemsFrom(modelSource);
            }
        }

        protected override void MakeViewModel()
        {
            LoadMealNames();
            CreateMealViewModel();
        }

        protected override string Validate()
        {
            return modelCopy.Validate();
        }

        private void LoadMealNames()
        {
            var loader = new MealListingViewModel.MealNamesAndMealsLoader(factories, true);
            MealNames = loader.MealNames;
            foreach (var mealName in MealNames)
            {
                mealName.MakeBuffer();
            }
            defaultMealName = loader.DefaultMealName;
            MealNames.Insert(0, defaultMealName);
        }

        private void CreateMealViewModel()
        {
            Meal = new MealViewModel(modelCopy)
            {
                MealNames = MealNames,
                DefaultMealName = defaultMealName
            };
            Meal.PropertyChanged += delegate { OnGotDirty(); };
        }

        private void SaveMealNames()
        {
            foreach (var mealName in MealNames)
            {
                mealName.FlushBuffer();
            }
            var models = factories.MealNames;
            foreach (var mealName in addedMealNames)
            {
                models.Add(mealName.Model);
            }
            foreach (var mealName in deletedMealNames)
            {
                models.Remove(mealName.Model);
            }
        }
    }
}
