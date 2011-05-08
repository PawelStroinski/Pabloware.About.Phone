using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Dietphone.ViewModels;
using Telerik.Windows.Controls;
using System.Diagnostics;
using Telerik.Windows.Data;

namespace Dietphone.Views
{
    public partial class ProductListing : UserControl
    {
        public ProductListingViewModel ViewModel { get; private set; }
        public event EventHandler CategoriesPoppedUp;
        private Guid topItemId;
        private bool topItemIsCategory;

        public ProductListing()
        {
            InitializeComponent();
            ViewModel = new ProductListingViewModel(App.Factories);
            DataContext = ViewModel;
            ViewModel.GroupDescriptors = List.GroupDescriptors;
            ViewModel.SortDescriptors = List.SortDescriptors;
            ViewModel.FilterDescriptors = List.FilterDescriptors;
            ViewModel.UpdateGroupDescriptors();
            ViewModel.UpdateSortDescriptors();
            ViewModel.BeginDataUpdate += new EventHandler(ViewModel_BeginDataUpdate);
            ViewModel.EndDataUpdate += new EventHandler(ViewModel_EndDataUpdate);
            ViewModel.BeforeRefresh += new EventHandler(ViewModel_BeforeRefresh);
            ViewModel.AfterRefresh += new EventHandler(ViewModel_AfterRefresh);
        }

        private void ViewModel_BeginDataUpdate(object sender, EventArgs e)
        {
            List.BeginDataUpdate();
        }

        private void ViewModel_EndDataUpdate(object sender, EventArgs e)
        {
            List.EndDataUpdate();
        }

        private void ViewModel_BeforeRefresh(object sender, EventArgs e)
        {
            MyStopwatch.Start();
            topItemId = Guid.Empty;
            var topItemSource = List.TopVisibleItem;
            if (topItemSource != null && topItemSource.Value != null)
            {
                var topItem = topItemSource.Value;
                if (topItem is ProductViewModel)
                {
                    var product = topItem as ProductViewModel;
                    topItemId = product.Id;
                    topItemIsCategory = false;
                }
                else
                    if (topItem is DataGroup)
                    {
                        var dataGroup = topItem as DataGroup;
                        if (dataGroup.Key is CategoryViewModel)
                        {
                            var category = dataGroup.Key as CategoryViewModel;
                            topItemId = category.Id;
                            topItemIsCategory = true;
                        }
                    }
            }
        }

        private void ViewModel_AfterRefresh(object sender, EventArgs e)
        {
            object topItem = null;
            if (topItemIsCategory)
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

        private void List_GroupPickerItemTap(object sender, Telerik.Windows.Controls.GroupPickerItemTapEventArgs e)
        {
            (sender as RadJumpList).UniversalGroupPickerItemTap(e);
        }

        private void List_GroupHeaderItemTap(object sender, Telerik.Windows.Controls.GroupHeaderItemTapEventArgs e)
        {
            if (CategoriesPoppedUp != null)
            {
                CategoriesPoppedUp(this, EventArgs.Empty);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            MyStopwatch.Stop();
        }
    }
}
