using System;
using System.Linq;
using System.Windows.Controls;
using Dietphone.ViewModels;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;
using Dietphone.Tools;

namespace Dietphone.Views
{
    public partial class MealListing : UserControl
    {
        public MealListingViewModel ViewModel { get; private set; }
        public event EventHandler DatesPoppedUp;
        private bool isTopItemMeal;
        private bool isTopItemDate;
        private Guid topItemMealId;
        private DateTime topItemDate;

        public MealListing()
        {
            InitializeComponent();
            ViewModel = new MealListingViewModel(App.Factories);
            DataContext = ViewModel;
            ViewModel.GroupDescriptors = List.GroupDescriptors;
            ViewModel.FilterDescriptors = List.FilterDescriptors;
            ViewModel.UpdateGroupDescriptors();
            ViewModel.DescriptorsUpdating += delegate { List.BeginDataUpdate(); };
            ViewModel.DescriptorsUpdated += delegate { List.EndDataUpdate(); };
            ViewModel.Refreshing += new EventHandler(ViewModel_Refreshing);
            ViewModel.Refreshed += new EventHandler(ViewModel_Refreshed);
        }

        private void ViewModel_Refreshing(object sender, EventArgs e)
        {
            isTopItemMeal = false;
            isTopItemDate = false;
            var topItemSource = List.TopVisibleItem;
            if (topItemSource != null && topItemSource.Value != null)
            {
                var topItem = topItemSource.Value;
                if (topItem is MealViewModel)
                {
                    var meal = topItem as MealViewModel;
                    topItemMealId = meal.Id;
                    isTopItemMeal = true;
                }
                else
                    if (topItem is DataGroup)
                    {
                        var dataGroup = topItem as DataGroup;
                        if (dataGroup.Key is DateViewModel)
                        {
                            var date = dataGroup.Key as DateViewModel;
                            topItemDate = date.Date;
                            isTopItemDate = true;
                        }
                    }
            }
        }

        private void ViewModel_Refreshed(object sender, EventArgs e)
        {
            object topItem = null;
            if (isTopItemMeal)
            {
                topItem = ViewModel.FindMeal(topItemMealId);
            }
            else
                if (isTopItemDate)
                {
                    var date = ViewModel.FindDate(topItemDate);
                    var group = from dataGroup in List.Groups
                                where dataGroup.Key == date
                                select dataGroup;
                    topItem = group.FirstOrDefault();
                }
            if (topItem != null)
            {
                List.BringIntoView(topItem);
            }
        }

        private void List_ItemTap(object sender, ListBoxItemTapEventArgs e)
        {
            var meal = List.SelectedItem as MealViewModel;
            if (meal != null)
            {
                ViewModel.Choose(meal);
            }
            Dispatcher.BeginInvoke(() =>
            {
                List.SelectedItem = null;
            });
        }

        private void List_GroupPickerItemTap(object sender, Telerik.Windows.Controls.GroupPickerItemTapEventArgs e)
        {
            (sender as RadJumpList).UniversalGroupPickerItemTap(e);
        }

        private void List_GroupHeaderItemTap(object sender, Telerik.Windows.Controls.GroupHeaderItemTapEventArgs e)
        {
            OnDatesPoppedUp();
        }

        protected void OnDatesPoppedUp()
        {
            if (DatesPoppedUp != null)
            {
                DatesPoppedUp(this, EventArgs.Empty);
            }
        }
    }
}
