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

namespace Dietphone.Views
{
    public partial class ProductListing : UserControl
    {
        public ProductListingViewModel ViewModel { get; private set; }
        public event EventHandler CategoriesPoppedUp;

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
            ViewModel.Invalidate += new EventHandler(ViewModel_Invalidate);
        }

        private void ViewModel_BeginDataUpdate(object sender, EventArgs e)
        {
            List.BeginDataUpdate();
        }

        private void ViewModel_EndDataUpdate(object sender, EventArgs e)
        {
            List.EndDataUpdate();
        }

        private void ViewModel_Invalidate(object sender, EventArgs e)
        {
            var top = List.TopVisibleItem;
            List.RefreshData();
            if (top != null && top.Value != null)
            {
                List.BringIntoView(top.Value);
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
    }
}
