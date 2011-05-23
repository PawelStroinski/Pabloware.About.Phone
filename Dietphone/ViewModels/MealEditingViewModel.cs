using Dietphone.Models;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Dietphone.Tools;
using System;

namespace Dietphone.ViewModels
{
    public class MealEditingViewModel : EditingViewModelBase<Meal>
    {
        public ObservableCollection<MealNameViewModel> Names { get; private set; }
        public MealViewModel Meal { get; private set; }
        private List<MealNameViewModel> addedNames = new List<MealNameViewModel>();
        private List<MealNameViewModel> deletedNames = new List<MealNameViewModel>();

        public MealEditingViewModel(Factories factories, Navigator navigator)
            : base(factories, navigator)
        {
        }

        public string NameName
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

        public void AddAndSetName(string name)
        {
            var tempModel = factories.CreateMealName();
            var models = factories.MealNames;
            models.Remove(tempModel);
            var viewModel = new MealNameViewModel(tempModel);
            viewModel.Name = name;
            Names.Add(viewModel);
            Meal.Name = viewModel;
            addedNames.Add(viewModel);
        }

        public void DeleteName()
        {
            var toDelete = Meal.Name;
            Meal.Name = Names.GetNextItemToSelectWhenDeleteSelected(toDelete);
            Names.Remove(toDelete);
            deletedNames.Add(toDelete);
        }

        public void SaveAndReturn()
        {
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
            LoadNames();
            CreateMealViewModel();
        }

        protected override string Validate()
        {
            return modelCopy.Validate();
        }

        private void LoadNames()
        {
            var loader = new MealListingViewModel.MealNamesAndMealsLoader(factories);
            loader.SortMealNames = true;
            Names = loader.GetMealNamesReloaded();
            foreach (var name in Names)
            {
                name.MakeBuffer();
            }
        }

        private void CreateMealViewModel()
        {
            Meal = new MealViewModel(modelCopy)
            {
                MealNames = Names
            };
            Meal.PropertyChanged += delegate { OnGotDirty(); };
        }

        private void SaveNames()
        {
            foreach (var name in Names)
            {
                name.FlushBuffer();
            }
            var models = factories.MealNames;
            foreach (var name in addedNames)
            {
                models.Add(name.Model);
            }
            foreach (var name in deletedNames)
            {
                models.Remove(name.Model);
            }
        }
    }
}
