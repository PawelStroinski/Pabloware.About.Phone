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
        private MealEditingViewModel viewModel;

        public MealEditing()
        {
            InitializeComponent();
            InteractionEffectManager.AllowedTypes.Add(typeof(RadDataBoundListBoxItem));
            Save = this.GetIcon(0);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var navigator = new NavigatorImpl(NavigationService, NavigationContext);
            viewModel = new MealEditingViewModel(App.Factories, navigator);
            DataContext = viewModel;
            viewModel.GotDirty += new EventHandler(viewModel_GotDirty);
            viewModel.CannotSave += new EventHandler<CannotSaveEventArgs>(viewModel_CannotSave);
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
                viewModel.AddAndSetMealName(input.Text);
                MealName.ForceRefresh(ProgressBar);
            };
        }

        private void EditMealName_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.CanRenameMealName())
            {
                RenameMealName();
            }
            else
            {
                MessageBox.Show(viewModel.NameOfMealName, "Nie można edytować tej nazwy.",
                    MessageBoxButton.OK);
            }
        }

        private void RenameMealName()
        {
            MealName.QuicklyCollapse();
            var input = new XnaInputBox(this)
            {
                Title = "EDYTUJ NAZWĘ",
                Description = "Nazwa",
                Text = viewModel.NameOfMealName
            };
            input.Show();
            input.Confirmed += delegate
            {
                viewModel.NameOfMealName = input.Text;
                MealName.ForceRefresh(ProgressBar);
            };
        }

        private void DeleteMealName_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.CanDeleteMealName())
            {
                DeleteMealName();
            }
            else
            {
                MessageBox.Show(viewModel.NameOfMealName, "Nie można usunąć tej nazwy.",
                    MessageBoxButton.OK);
            }
        }

        private void DeleteMealName()
        {
            if (MessageBox.Show(
                String.Format("Czy na pewno chcesz trwale usunąć tę nazwę?\r\n\r\n{0}",
                viewModel.NameOfMealName),
                "Usunąć nazwę?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                Save.IsEnabled = false;
                MealName.QuicklyCollapse();
                Dispatcher.BeginInvoke(() =>
                {
                    viewModel.DeleteMealName();
                    MealName.ForceRefresh(ProgressBar);
                });
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            Focus();
            Dispatcher.BeginInvoke(() =>
            {
                if (viewModel.CanSave())
                {
                    viewModel.UpdateTimeAndSaveAndReturn();
                }
            });
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            viewModel.CancelAndReturn();
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                String.Format("Czy na pewno chcesz trwale usunąć ten posiłek?\r\n\r\n{0}",
                viewModel.IdentifiableName),
                "Usunąć posiłek?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                viewModel.DeleteAndSaveAndReturn();
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

        }

        private void Items_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Items.SelectedItem != null)
            {
                MealItemEditing.Picker.IsPopupOpen = true;
            }
            Items.SelectedItem = null;
        }
    }
}