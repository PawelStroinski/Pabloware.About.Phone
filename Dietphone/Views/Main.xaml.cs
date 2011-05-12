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
using Microsoft.Phone.Controls;
using System.ComponentModel;
using Dietphone.ViewModels;
using System.Diagnostics;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;
using System.Windows.Navigation;
using Dietphone.Tools;

namespace Dietphone.Views
{
    public partial class Main : PhoneApplicationPage
    {
        public MainViewModel ViewModel { get; private set; }
        private SubViewModelConnector subConnector;
        private bool searchShowed;
        private const byte BACK_KEY = 27;

        public Main()
        {
            InitializeComponent();
            ViewModel = new MainViewModel();
            DataContext = ViewModel;
            subConnector = new SubViewModelConnector(ViewModel);
        }

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            MealListItems.DataSource = ViewModel.Meals;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var navigator = new NavigatorImpl(NavigationService, NavigationContext);
            subConnector.Navigator = navigator;
            subConnector.Refresh();
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Pivot.SelectedItem == Products)
            {
                subConnector.SubViewModel = ProductListing.ViewModel;
            }
        }

        private void SearchIcon_Click(object sender, EventArgs e)
        {
            if (searchShowed)
            {
                HideOrFocusSearch();
            }
            else
            {
                ShowSearch();
            }
        }

        private void Main_BackKeyPress(object sender, CancelEventArgs e)
        {
            if (searchShowed)
            {
                HideSearch();
                e.Cancel = true;
            }
        }

        private void SearchBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.PlatformKeyCode == BACK_KEY)
            {
                HideSearch();
            }
        }

        private void ProductListing_CategoriesPoppedUp(object sender, EventArgs e)
        {
            HideSearch();
        }

        private void ProductListing_MouseEnter(object sender, MouseEventArgs e)
        {
            HideSearchSip();
        }

        private void HideOrFocusSearch()
        {
            if (searchShowed)
            {
                if (SearchBox.IsFocused())
                {
                    HideSearch();
                }
                else
                {
                    SearchBox.Focus();
                    SearchBox.SelectAll();
                }
            }
        }

        private void HideSearch()
        {
            if (searchShowed)
            {
                searchShowed = false;
                HideSearchAnimation.Begin();
                HideSearchAnimation.Completed += (Sender, E) =>
                {
                    SearchBorder.Visibility = Visibility.Collapsed;
                    SearchBox.Text = "";
                };
            }
        }

        private void ShowSearch()
        {
            var searchHidden = !searchShowed;
            if (searchHidden)
            {
                searchShowed = true;
                SearchBorder.Visibility = Visibility.Visible;
                ShowSearchAnimation.Begin();
                SearchBox.Focus();
            }
        }

        private void HideSearchSip()
        {
            var sipVisible = SearchBox.IsFocused();
            if (sipVisible)
            {
                Focus();
            }
        }
    }
}