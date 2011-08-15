using System;
using System.Windows;
using Microsoft.Phone.Controls;
using Dietphone.ViewModels;
using Telerik.Windows.Controls;
using Dietphone.Tools;
using System.Windows.Navigation;
using System.Windows.Controls;
using System.Windows.Input;

namespace Dietphone.Views
{
    public partial class MealEditing : PhoneApplicationPage
    {
        public MealEditingViewModel ViewModel { get; private set; }

        public MealEditing()
        {
            InitializeComponent();
            InteractionEffectManager.AllowedTypes.Add(typeof(RadDataBoundListBoxItem));
            Save = this.GetIcon(0);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ViewModel == null)
            {
                var navigator = new NavigatorImpl(NavigationService, NavigationContext);
                ViewModel = new MealEditingViewModel(MyApp.Factories, navigator);
                DataContext = ViewModel;
                ViewModel.GotDirty += viewModel_GotDirty;
                ViewModel.CannotSave += viewModel_CannotSave;
                ViewModel.ItemEditing = ItemEditing.ViewModel;
                ViewModel.InvalidateItems += ViewModel_InvalidateItems;
            }
            else
            {
                ViewModel.ReturnedFromNavigation();
            }
        }

        private void AddMealName_Click(object sender, RoutedEventArgs e)
        {
            MealName.QuicklyCollapse();
            var input = new XnaInputBox(this)
            {
                Title = "DODAJ NAZWĘ",
                Description = "Nazwa"
            };
            input.Show();
            input.Confirmed += delegate
            {
                ViewModel.AddAndSetName(input.Text);
                MealName.ForceRefresh(ProgressBar);
            };
        }

        private void EditMealName_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CanEditName())
            {
                EditMealNameDo();
            }
            else
            {
                MessageBox.Show(ViewModel.NameOfName, "Nie można edytować tej nazwy.",
                    MessageBoxButton.OK);
            }
        }

        private void EditMealNameDo()
        {
            MealName.QuicklyCollapse();
            var input = new XnaInputBox(this)
            {
                Title = "EDYTUJ NAZWĘ",
                Description = "Nazwa",
                Text = ViewModel.NameOfName
            };
            input.Show();
            input.Confirmed += delegate
            {
                ViewModel.NameOfName = input.Text;
                MealName.ForceRefresh(ProgressBar);
            };
        }

        private void DeleteMealName_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CanDeleteName())
            {
                DeleteMealNameDo();
            }
            else
            {
                MessageBox.Show(ViewModel.NameOfName, "Nie można usunąć tej nazwy.",
                    MessageBoxButton.OK);
            }
        }

        private void DeleteMealNameDo()
        {
            if (MessageBox.Show(
                String.Format("Czy na pewno chcesz trwale usunąć tę nazwę?\r\n\r\n{0}",
                ViewModel.NameOfName),
                "Usunąć nazwę?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                Save.IsEnabled = false;
                MealName.QuicklyCollapse();
                Dispatcher.BeginInvoke(() =>
                {
                    ViewModel.DeleteName();
                    MealName.ForceRefresh(ProgressBar);
                });
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            Focus();
            Dispatcher.BeginInvoke(() =>
            {
                if (ViewModel.CanSave())
                {
                    ViewModel.UpdateTimeAndSaveAndReturn();
                }
            });
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            ViewModel.CancelAndReturn();
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                String.Format("Czy na pewno chcesz trwale usunąć ten posiłek?\r\n\r\n{0}",
                ViewModel.IdentifiableName),
                "Usunąć posiłek?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                ViewModel.DeleteAndSaveAndReturn();
            }
        }

        private void viewModel_GotDirty(object sender, EventArgs e)
        {
            Save.IsEnabled = true;
        }

        private void viewModel_CannotSave(object sender, CannotSaveEventArgs e)
        {
            e.Ignore = (MessageBox.Show(e.Reason, "Czy na pewno chcesz zapisać ten posiłek?",
                MessageBoxButton.OKCancel) == MessageBoxResult.OK);
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddItem();
        }

        private void Items_ItemTap(object sender, ListBoxItemTapEventArgs e)
        {
            var item = Items.SelectedItem as MealItemViewModel;
            if (item != null)
            {
                ViewModel.EditItem(item);
            }
            Items.SelectedItem = null;
        }

        private void ItemsGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var itemsGrid = (Grid)sender;
            var meal = ViewModel.Meal;
            var scores = meal.Scores;
            if (!scores.FirstExists)
            {
                itemsGrid.HideColumnWithIndex(1);
            }
            if (!scores.SecondExists)
            {
                itemsGrid.HideColumnWithIndex(2);
            }
            if (!scores.ThirdExists)
            {
                itemsGrid.HideColumnWithIndex(3);
            }
            if (!scores.FourthExists)
            {
                itemsGrid.HideColumnWithIndex(4);
            }
        }

        private void Score_Click(object sender, MouseButtonEventArgs e)
        {
            ViewModel.ScoresSettings();
        }

        private void ViewModel_InvalidateItems(object sender, EventArgs e)
        {
            Items.ForceInvalidate();
        }
    }
}