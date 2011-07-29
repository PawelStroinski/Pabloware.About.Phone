using System;
using System.Linq;
using System.Windows.Controls;
using Dietphone.ViewModels;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;
using Dietphone.Tools;

namespace Dietphone.Views
{
    public partial class ProductListing : UserControl
    {
        public ProductListingViewModel ViewModel { get; private set; }
        public event EventHandler CategoriesPoppedUp;
        private bool isTopItemCategory;
        private Guid topItemId;

        public ProductListing()
        {
            InitializeComponent();
            ViewModel = new ProductListingViewModel(MyApp.Factories);
            DataContext = ViewModel;
            ViewModel.GroupDescriptors = List.GroupDescriptors;
            ViewModel.SortDescriptors = List.SortDescriptors;
            ViewModel.FilterDescriptors = List.FilterDescriptors;
            ViewModel.UpdateGroupDescriptors();
            ViewModel.UpdateSortDescriptors();
            ViewModel.DescriptorsUpdating += delegate { List.BeginDataUpdate(); };
            ViewModel.DescriptorsUpdated += delegate { List.EndDataUpdate(); };
            ViewModel.Refreshing += new EventHandler(ViewModel_Refreshing);
            ViewModel.Refreshed += new EventHandler(ViewModel_Refreshed);
        }

        private void ViewModel_Refreshing(object sender, EventArgs e)
        {
            topItemId = Guid.Empty;
            var topItemSource = List.TopVisibleItem;
            if (topItemSource != null && topItemSource.Value != null)
            {
                var topItem = topItemSource.Value;
                if (topItem is ProductViewModel)
                {
                    var product = topItem as ProductViewModel;
                    topItemId = product.Id;
                    isTopItemCategory = false;
                }
                else
                    if (topItem is DataGroup)
                    {
                        var dataGroup = topItem as DataGroup;
                        if (dataGroup.Key is CategoryViewModel)
                        {
                            var category = dataGroup.Key as CategoryViewModel;
                            topItemId = category.Id;
                            isTopItemCategory = true;
                        }
                    }
            }
        }

        private void ViewModel_Refreshed(object sender, EventArgs e)
        {
            object topItem = null;
            if (isTopItemCategory)
            {
                var category = ViewModel.FindCategory(topItemId);
                var group = from dataGroup in List.Groups
                            where dataGroup.Key == category
                            select dataGroup;
                topItem = group.FirstOrDefault();
            }
            else
            {
                topItem = ViewModel.FindProduct(topItemId);
            }
            if (topItem != null)
            {
                List.BringIntoView(topItem);
            }
        }

        private void List_ItemTap(object sender, ListBoxItemTapEventArgs e)
        {
            var product = List.SelectedItem as ProductViewModel;
            if (product != null)
            {
                ViewModel.Choose(product);
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
            OnCategoriesPoppedUp();
        }

        protected void OnCategoriesPoppedUp()
        {
            if (CategoriesPoppedUp != null)
            {
                CategoriesPoppedUp(this, EventArgs.Empty);
            }
        }
    }
}
