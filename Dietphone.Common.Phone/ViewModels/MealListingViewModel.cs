using Dietphone.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Telerik.Windows.Data;
using System.Collections.Generic;

namespace Dietphone.ViewModels
{
    public class MealListingViewModel : SubViewModel
    {
        public ObservableCollection<MealViewModel> Meals { get; private set; }
        public ObservableCollection<DateViewModel> Dates { get; private set; }
        public ObservableCollection<DataDescriptor> GroupDescriptors { private get; set; }
        public ObservableCollection<DataDescriptor> FilterDescriptors { private get; set; }
        public event EventHandler DescriptorsUpdating;
        public event EventHandler DescriptorsUpdated;
        private Factories factories;

        public MealListingViewModel(Factories factories)
        {
            this.factories = factories;
        }

        public override void Load()
        {
            if (Dates == null && Meals == null)
            {
                var loader = new NamesAndMealsLoader(this);
                loader.LoadAsync();
                loader.Loaded += delegate { OnLoaded(); };
            }
        }

        public override void Refresh()
        {
            if (Dates != null && Meals != null)
            {
                var loader = new NamesAndMealsLoader(this);
                loader.LoadAsync();
                loader.Loaded += delegate { OnRefreshed(); };
            }
        }

        public void Choose(MealViewModel meal)
        {
            Navigator.GoToMealEditing(meal.Id);
        }

        public override void Add()
        {
            var meal = factories.CreateMeal();
            Navigator.GoToMealEditing(meal.Id);
        }

        public void UpdateGroupDescriptors()
        {
            GroupDescriptors.Clear();
            var groupByDate = new GenericGroupDescriptor<MealViewModel, DateViewModel>(meal => meal.Date);
            GroupDescriptors.Add(groupByDate);
        }

        public MealViewModel FindMeal(Guid mealId)
        {
            var result = from meal in Meals
                         where meal.Id == mealId
                         select meal;
            return result.FirstOrDefault();
        }

        public DateViewModel FindDate(DateTime value)
        {
            var result = from date in Dates
                         where date.Date == value
                         select date;
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

        public class NamesAndMealsLoader : LoaderBase
        {
            private ObservableCollection<MealNameViewModel> names;
            private List<MealViewModel> unsortedMeals;
            private ObservableCollection<MealViewModel> sortedMeals;
            private ObservableCollection<DateViewModel> dates;
            private MealNameViewModel defaultName;
            private readonly bool sortNames;
            private const byte DATES_MAX_COUNT = 14 * 3;

            public NamesAndMealsLoader(MealListingViewModel viewModel)
            {
                this.viewModel = viewModel;
                factories = viewModel.factories;
            }

            public NamesAndMealsLoader(Factories factories, bool sortNames)
            {
                this.factories = factories;
                this.sortNames = sortNames;
            }

            public ObservableCollection<MealNameViewModel> Names
            {
                get
                {
                    if (names == null)
                    {
                        LoadNames();
                    }
                    return names;
                }
            }

            public MealNameViewModel DefaultName
            {
                get
                {
                    if (defaultName == null)
                    {
                        MakeDefaultName();
                    }
                    return defaultName;
                }
            }

            protected override void DoWork()
            {
                LoadNames();
                MakeDefaultName();
                LoadUnsortedMeals();
                MakeDatesAndSortMeals();
            }

            protected override void WorkCompleted()
            {
                AssignDates();
                AssignSortedMeals();
                base.WorkCompleted();
            }

            private void LoadNames()
            {
                var models = factories.MealNames;
                var unsortedViewModels = new ObservableCollection<MealNameViewModel>();
                foreach (var model in models)
                {
                    var viewModel = new MealNameViewModel(model, factories);
                    unsortedViewModels.Add(viewModel);
                }
                if (sortNames)
                {
                    var sortedViewModels = unsortedViewModels.OrderBy(mealName => mealName.Name);
                    names = new ObservableCollection<MealNameViewModel>();
                    foreach (var viewModel in sortedViewModels)
                    {
                        names.Add(viewModel);
                    }
                }
                else
                {
                    names = unsortedViewModels;
                }
            }

            private void MakeDefaultName()
            {
                var defaultEntities = factories.DefaultEntities;
                var model = defaultEntities.MealName;
                defaultName = new MealNameViewModel(model, factories);
            }

            private void LoadUnsortedMeals()
            {
                var models = factories.Meals;
                unsortedMeals = new List<MealViewModel>();
                foreach (var model in models)
                {
                    var viewModel = new MealViewModel(model, factories)
                    {
                        Names = names,
                        DefaultName = defaultName
                    };
                    unsortedMeals.Add(viewModel);
                }
            }

            private void MakeDatesAndSortMeals()
            {
                var mealDatesDescending = from meal in unsortedMeals
                                          group meal by meal.DateOnly into date
                                          orderby date.Key descending
                                          select date;
                var newerCount = DATES_MAX_COUNT;
                if (mealDatesDescending.Count() > newerCount)
                {
                    newerCount--;
                }
                var newer = mealDatesDescending.Take(newerCount);
                var older = from date in mealDatesDescending.Skip(newerCount)
                            from meal in date
                            orderby meal.DateTime descending
                            select meal;
                dates = new ObservableCollection<DateViewModel>();
                sortedMeals = new ObservableCollection<MealViewModel>();
                foreach (var date in newer)
                {
                    var normalDate = DateViewModel.CreateNormalDate(date.Key);
                    dates.Add(normalDate);
                    var dateAscending = date.OrderBy(meal => meal.DateTime);
                    foreach (var meal in dateAscending)
                    {
                        meal.Date = normalDate;
                        sortedMeals.Add(meal);
                    }
                }
                if (older.Count() > 0)
                {
                    var groupOfOlder = DateViewModel.CreateGroupOfOlder();
                    dates.Add(groupOfOlder);
                    foreach (var meal in older)
                    {
                        meal.Date = groupOfOlder;
                        sortedMeals.Add(meal);
                    }
                }
            }

            private void AssignSortedMeals()
            {
                GetViewModel().Meals = sortedMeals;
                GetViewModel().OnPropertyChanged("Meals");
            }

            private void AssignDates()
            {
                GetViewModel().Dates = dates;
                GetViewModel().OnPropertyChanged("Dates");
            }

            private MealListingViewModel GetViewModel()
            {
                return viewModel as MealListingViewModel;
            }
        }
    }
}
