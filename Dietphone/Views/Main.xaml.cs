﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using System.ComponentModel;
using Dietphone.ViewModels;
using System.Windows.Navigation;
using Dietphone.Tools;
using Dietphone.Models;
using System.Windows.Threading;

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
            ViewModel = new MainViewModel(MyApp.Factories)
            {
                ProductListing = ProductListing.ViewModel,
                MealItemEditing = MealItemEditing.ViewModel
            };
            ViewModel.ShowProductsOnly += ViewModel_ShowProductsOnly;
            ViewModel.GoingToSettings += ViewModel_GoingToSettings;
            DataContext = ViewModel;
            subConnector = new SubViewModelConnector(ViewModel);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var navigator = new NavigatorImpl(NavigationService, NavigationContext);
            subConnector.Navigator = navigator;
            subConnector.Refresh();
            ViewModel.Navigator = navigator;
            SettingsIfFirstRunOnTimer();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.Content is MealEditing)
            {
                var viewModel = (e.Content as MealEditing).ViewModel;
                if (viewModel != null)
                {
                    ViewModel.MealEditing = viewModel;
                    ViewModel.ReturningToMealEditing();
                }
            }
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Pivot.SelectedItem == Meals)
            {
                subConnector.SubViewModel = MealListing.ViewModel;
            }
            else
                if (Pivot.SelectedItem == Products)
                {
                    subConnector.SubViewModel = ProductListing.ViewModel;
                }
        }

        private void Add_Click(object sender, EventArgs e)
        {
            subConnector.Add();
        }

        private void ViewModel_ShowProductsOnly(object sender, EventArgs e)
        {
            Pivot.Items.Remove(Meals);
        }

        private void ViewModel_GoingToSettings(object sender, EventArgs e)
        {
            MessageBox.Show("Dostosuj teraz Dietphone do siebie.",
                "Witaj w Dietphone!", MessageBoxButton.OK);
        }

        private void About_Click(object sender, EventArgs e)
        {
            ViewModel.About();
        }

        private void ExportAndImport_Click(object sender, EventArgs e)
        {
            ViewModel.ExportAndImport();
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            ViewModel.Settings();
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

        private void MealListing_DatesPoppedUp(object sender, EventArgs e)
        {
            HideSearch();
        }

        private void ProductListing_MouseEnter(object sender, MouseEventArgs e)
        {
            HideSearchSip();
        }

        private void MealListing_MouseEnter(object sender, MouseEventArgs e)
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

        private void SettingsIfFirstRunOnTimer()
        {
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.1);
            timer.Tick += delegate
            {
                ViewModel.SettingsIfFirstRun();
            };
            timer.Start();
        }
    }
}