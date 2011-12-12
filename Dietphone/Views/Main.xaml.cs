using Dietphone.Tools;
using Dietphone.ViewModels;
using Microsoft.Phone.Controls;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using TombstoneHelper;
using System.Collections.Generic;

namespace Dietphone.Views
{
    public partial class Main : PhoneApplicationPage, StateProvider
    {
        public MainViewModel ViewModel { get; private set; }
        private SubViewModelConnector subConnector;
        private bool searchShowed;
        private bool searchFocused;
        private bool alreadyRestoredSearch;
        private const byte BACK_KEY = 27;
        private const string SEARCH = "SEARCH";
        private const string SEARCH_SHOWED = "SEARCH_SHOWED";
        private const string SEARCH_FOCUSED = "SEARCH_FOCUSED";

        public Main()
        {
            InitializeComponent();
            ViewModel = new MainViewModel(MyApp.Factories)
            {
                ProductListing = ProductListing.ViewModel,
                MealItemEditing = MealItemEditing.ViewModel,
                StateProvider = this
            };
            ViewModel.ShowProductsOnly += ViewModel_ShowProductsOnly;
            DataContext = ViewModel;
            subConnector = new SubViewModelConnector(ViewModel);
            subConnector.Loaded += delegate { RestoreSearchUi(); };
            subConnector.Refreshed += delegate { RestoreSearchUi(); };
            TranslateApplicationBar();
            MealListing.StateProvider = this;
            ProductListing.StateProvider = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.RestoreState();
            // We need to restore pivot manually because TombstoneHelper will wait for
            // event Pivot.Loaded which in this case is fired after page is shown to user.
            // TombstoneHelper works this way probably because was bug in Pivot in WP 7.0.
            Pivot.UntombstoneWith(this);
            UntombstoneSearchNoRestoreUi();
            var navigator = new NavigatorImpl(NavigationService, NavigationContext);
            subConnector.Navigator = navigator;
            subConnector.Refresh();
            ViewModel.Navigator = navigator;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.Content is MealEditing)
            {
                var mealEditing = (e.Content as MealEditing).ViewModel;
                ViewModel.MealEditing = mealEditing;
                ViewModel.GoingToMealEditing();
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            this.SaveState(e);
            if (e.NavigationMode != NavigationMode.Back)
            {
                MealListing.Tombstone();
                ProductListing.Tombstone();
                TombstoneSearchBeforeExit();
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
                FocusSearch();
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

        private void TombstoneSearchBeforeExit()
        {
            TombstoneSearchInternal();
            HideSearchUiBeforeRestore();
            alreadyRestoredSearch = false;
        }

        private void UntombstoneSearchNoRestoreUi()
        {
            if (State.ContainsKey(SEARCH))
            {
                ViewModel.Search = (string)State[SEARCH];
                searchShowed = (bool)State[SEARCH_SHOWED];
                searchFocused = (bool)State[SEARCH_FOCUSED];
            }
        }

        private void RestoreSearchUi()
        {
            if (!alreadyRestoredSearch)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    RestoreSearchUiInternal();
                });
                alreadyRestoredSearch = true;
            }
        }

        private void TombstoneSearchInternal()
        {
            SaveSearchInternal();
            State[SEARCH] = ViewModel.Search;
            State[SEARCH_SHOWED] = searchShowed;
            State[SEARCH_FOCUSED] = searchFocused;
        }

        private void SaveSearchInternal()
        {
            searchFocused = SearchBox.IsFocused();
        }

        private void RestoreSearchUiInternal()
        {
            if (searchShowed)
            {
                searchShowed = false;
                ShowSearch();
                if (searchFocused)
                {
                    FocusSearch();
                }
            }
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

        private void HideSearchUiBeforeRestore()
        {
            if (searchShowed)
            {
                HideSearchAnimation.Begin();
                HideSearchAnimation.SkipToFill();
                SearchBorder.Visibility = Visibility.Collapsed;
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
            }
        }

        private void FocusSearch()
        {
            if (searchShowed)
            {
                SearchBox.Focus();
                var text = SearchBox.Text;
                var textLength = text.Length;
                SearchBox.Select(textLength, 0);
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

        private void TranslateApplicationBar()
        {
            this.GetIcon(0).Text = Translations.Add;
            this.GetIcon(1).Text = Translations.Search;
            this.GetMenuItem(0).Text = Translations.Settings;
            this.GetMenuItem(1).Text = Translations.ExportAndImportData;
            this.GetMenuItem(2).Text = Translations.About;
        }
    }
}