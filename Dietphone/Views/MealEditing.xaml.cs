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
using Dietphone.ViewModels;
using Dietphone.Models;
using Telerik.Windows.Controls;
using System.Windows.Controls.Primitives;
using Dietphone.Tools;
using System.Windows.Navigation;

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
                ViewModel = new MealEditingViewModel(App.Factories, navigator);
                DataContext = ViewModel;
                ViewModel.GotDirty += new EventHandler(viewModel_GotDirty);
                ViewModel.CannotSave += new EventHandler<CannotSaveEventArgs>(viewModel_CannotSave);
                ViewModel.ItemEditing = ItemEditing.ViewModel;
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
                EditName();
            }
            else
            {
                MessageBox.Show(ViewModel.NameOfName, "Nie można edytować tej nazwy.",
                    MessageBoxButton.OK);
            }
        }

        private void EditName()
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
                DeleteMealName();
            }
            else
            {
                MessageBox.Show(ViewModel.NameOfName, "Nie można usunąć tej nazwy.",
                    MessageBoxButton.OK);
            }
        }

        private void DeleteMealName()
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
    }
}