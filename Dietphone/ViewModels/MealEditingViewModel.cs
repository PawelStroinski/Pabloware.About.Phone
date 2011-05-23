using Dietphone.Models;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Dietphone.Tools;
using System;

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

        public List<string> AllServingSizeUnits
        {
            get
            {
                return UnitAbbreviations.GetAll();
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

        public void DeleteName()
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
            foreach (var name in MealNames)
            {
                name.MakeBuffer();
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
            foreach (var name in MealNames)
            {
                name.FlushBuffer();
            }
            var models = factories.MealNames;
            foreach (var name in addedMealNames)
            {
                models.Add(name.Model);
            }
            foreach (var name in deletedMealNames)
            {
                models.Remove(name.Model);
            }
        }
    }
}
