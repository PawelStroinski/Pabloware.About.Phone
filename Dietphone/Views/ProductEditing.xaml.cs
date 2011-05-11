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
using System.Windows.Navigation;
using Dietphone.Tools;
using Telerik.Windows.Controls;
using System.ComponentModel;
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls.Primitives;

namespace Dietphone.Views
{
    public partial class ProductEditing : PhoneApplicationPage
    {
        private ProductEditingViewModel viewModel;

        public ProductEditing()
        {
            InitializeComponent();
            SaveIcon = GetIcon(0);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var navigator = new NavigatorImpl(NavigationService, NavigationContext);
            viewModel = new ProductEditingViewModel(App.Factories, navigator);
            DataContext = viewModel;
            viewModel.GotDirty += new EventHandler(viewModel_GotDirty);
            viewModel.CannotSave += new EventHandler<CannotSaveEventArgs>(viewModel_CannotSave);
        }

        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            Categories.IsExpanded = false;
            var input = new XnaInputBox(this)
            {
                Title = "DODAJ KATEGORIĘ",
                Description = "Nazwa"
            };
            input.Show();
            input.Confirmed += delegate
            {
                viewModel.AddAndSetCategory(input.Text);
                Categories.ForceRefresh(ProgressBar);
            };
        }

        private void EditCategory_Click(object sender, RoutedEventArgs e)
        {
            Categories.IsExpanded = false;
            var input = new XnaInputBox(this)
            {
                Title = "EDYTUJ KATEGORIĘ",
                Description = "Nazwa",
                Text = viewModel.CategoryName
            };
            input.Show();
            input.Confirmed += delegate
            {
                viewModel.CategoryName = input.Text;
                Categories.ForceRefresh(ProgressBar);
            };
        }

        private void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.CanDeleteCategory())
            {
                DeleteCategory();
            }
            else
            {
                MessageBox.Show("Do tej kategorii należą inne produkty. " +
                    "Zmień ich kategorię i spróbuj ponownie.",
                    "Nie można usunąć", MessageBoxButton.OK);
            }
        }

        private void DeleteCategory()
        {
            if (MessageBox.Show(
                String.Format("Czy na pewno chcesz trwale usunąć tę kategorię?\r\n\r\n{0}",
                viewModel.CategoryName),
                "Usunąć kategorię?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                SaveIcon.IsEnabled = false;
                Categories.IsExpanded = false;
                Dispatcher.BeginInvoke(() =>
                {
                    viewModel.DeleteCategory();
                    Categories.ForceRefresh(ProgressBar);
                });
            }
        }

        private void SaveIcon_Click(object sender, EventArgs e)
        {
            if (viewModel.CanSave())
            {
                viewModel.SaveAndReturn();
            }
        }

        private void CancelIcon_Click(object sender, EventArgs e)
        {
            viewModel.CancelAndReturn();
        }

        private void viewModel_GotDirty(object sender, EventArgs e)
        {
            SaveIcon.IsEnabled = true;
        }

        private void viewModel_CannotSave(object sender, CannotSaveEventArgs e)
        {
            e.Ignore = (MessageBox.Show(e.Reason, "Czy na pewno chcesz zapisać ten produkt?",
                MessageBoxButton.OKCancel) == MessageBoxResult.OK);
        }

        private ApplicationBarIconButton GetIcon(int whichIcon)
        {
            return ApplicationBar.Buttons[whichIcon] as ApplicationBarIconButton;
        }

        private void Categories_ItemClick(object sender, SelectorItemClickEventArgs e)
        {
            Vibration vibration = new VibrationImpl();
            vibration.VibrateOnButtonPress();
        }
    }
}