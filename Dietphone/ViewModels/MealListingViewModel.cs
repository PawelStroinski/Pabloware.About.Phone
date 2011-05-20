using Dietphone.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Telerik.Windows.Data;

namespace Dietphone.ViewModels
{
    public class MealListingViewModel : SubViewModel
    {
        public ObservableCollection<MealViewModel> Meals { get; private set; }
        public ObservableCollection<DataDescriptor> GroupDescriptors { private get; set; }
        public ObservableCollection<DataDescriptor> SortDescriptors { private get; set; }
        public ObservableCollection<DataDescriptor> FilterDescriptors { private get; set; }
        public event EventHandler DescriptorsUpdating;
        public event EventHandler DescriptorsUpdated;
        private Factories factories;
        private MealViewModel selectedMeal;

        public MealListingViewModel(Factories factories)
        {
            this.factories = factories;
        }

        public MealViewModel SelectedMeal
        {
            get
            {
                return selectedMeal;
            }
            set
            {
                if (selectedMeal != value)
                {
                    selectedMeal = value;
                    OnSelectedMealChanged();
                }
            }
        }

        public override void Load()
        {
            if (Meals == null)
            {
                var loader = new MealNamesAndMealsLoader(this);
                loader.LoadAsync();
            }
        }

        public override void Refresh()
        {
            OnRefreshing();
            var loader = new MealNamesAndMealsLoader(this);
            loader.LoadAsync();
            loader.Loaded += delegate { OnRefreshed(); };
        }

        public override void Add()
        {
            var meal = factories.CreateMeal();
            Navigator.GoToMealEditing(meal.Id);
        }

        public void UpdateGroupDescriptors()
        {
            GroupDescriptors.Clear();
            var groupByDate = new GenericGroupDescriptor<MealViewModel, DateTime>(meal => meal.Date);
            GroupDescriptors.Add(groupByDate);
        }

        public void UpdateSortDescriptors()
        {
            SortDescriptors.Clear();
            var sortByDateTime = new GenericSortDescriptor<MealViewModel, DateTime>(meal => meal.DateTime);
            SortDescriptors.Add(sortByDateTime);
        }

        public MealViewModel FindMeal(Guid mealId)
        {
            var result = from meal in Meals
                         where meal.Id == mealId
                         select meal;
            return result.FirstOrDefault();
        }

        protected override void OnSearchChanged()
        {
            OnDescriptorsUpdating();
            UpdateFilterDescriptors();
            OnDescriptorsUpdated();
        }

        private void UpdateFilterDescriptors()
        {
            FilterDescriptors.Clear();
            if (!string.IsNullOrEmpty(search))
            {
                var filterIn = new GenericFilterDescriptor<MealViewModel>(meal => meal.FilterIn(search));
                FilterDescriptors.Add(filterIn);
            }
        }

        protected void OnSelectedMealChanged()
        {
            if (SelectedMeal != null)
            {
                Navigator.GoToMealEditing(SelectedMeal.Id);
            }
        }

        protected void OnDescriptorsUpdating()
        {
            if (DescriptorsUpdating != null)
            {
                DescriptorsUpdating(this, EventArgs.Empty);
            }
        }

        protected void OnDescriptorsUpdated()
        {
            if (DescriptorsUpdated != null)
            {
                DescriptorsUpdated(this, EventArgs.Empty);
            }
        }

        public class MealNamesAndMealsLoader : LoaderBase
        {
            public bool SortMealNames { get; set; }
            private ObservableCollection<MealNameViewModel> mealNames;
            private ObservableCollection<MealViewModel> meals;

            public MealNamesAndMealsLoader(MealListingViewModel viewModel)
            {
                this.viewModel = viewModel;
                factories = viewModel.factories;
            }

            public MealNamesAndMealsLoader(Factories factories)
            {
                this.factories = factories;
            }

            public ObservableCollection<MealNameViewModel> GetMealNamesReloaded()
            {
                LoadMealNames();
                return mealNames;
            }

            protected override void DoWork()
            {
                LoadMealNames();
                LoadMeals();
            }

            protected override void WorkCompleted()
            {
                AssignMeals();
                base.WorkCompleted();
            }

            private void LoadMealNames()
            {
                var models = factories.MealNames;
                var unsortedViewModels = new ObservableCollection<MealNameViewModel>();
                foreach (var model in models)
                {
                    var viewModel = new MealNameViewModel(model);
                    unsortedViewModels.Add(viewModel);
                }
                if (SortMealNames)
                {
                    var sortedViewModels = unsortedViewModels.OrderBy(mealName => mealName.Name);
                    mealNames = new ObservableCollection<MealNameViewModel>();
                    foreach (var viewModel in sortedViewModels)
                    {
                        mealNames.Add(viewModel);
                    }
                }
                else
                {
                    mealNames = unsortedViewModels;
                }
            }

            private void LoadMeals()
            {
                var models = factories.Meals;
                meals = new ObservableCollection<MealViewModel>();
                foreach (var model in models)
                {
                    var viewModel = new MealViewModel(model, mealNames);
                    meals.Add(viewModel);
                }
            }

            private void AssignMeals()
            {
                GetViewModel().Meals = meals;
                GetViewModel().OnPropertyChanged("Meals");
            }

            private MealListingViewModel GetViewModel()
            {
                return viewModel as MealListingViewModel;
            }
        }
    }
}
