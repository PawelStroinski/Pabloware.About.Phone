using System;
using System.Windows;
using Microsoft.Phone.Controls;
using Dietphone.ViewModels;
using System.Windows.Navigation;
using Dietphone.Tools;
using Telerik.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Dietphone.Views
{
    public partial class ProductEditing : PhoneApplicationPage
    {
        private ProductEditingViewModel viewModel;

        public ProductEditing()
        {
            InitializeComponent();
            Save = this.GetIcon(0);
            Loaded += new RoutedEventHandler(ProductEditing_Loaded);
            TranslateApplicationBar();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var navigator = new NavigatorImpl(NavigationService, NavigationContext);
            viewModel = new ProductEditingViewModel(MyApp.Factories, navigator);
            DataContext = viewModel;
            viewModel.GotDirty += new EventHandler(viewModel_GotDirty);
            viewModel.CannotSave += new EventHandler<CannotSaveEventArgs>(viewModel_CannotSave);
        }

        private void ProductEditing_Loaded(object sender, RoutedEventArgs e)
        {
            var product = viewModel.Product;
            if (string.IsNullOrEmpty(product.Name))
            {
                NameBox.Focus();
            }
        }

        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            Category.IsExpanded = false;
            var input = new XnaInputBox(this)
            {
                Title = Translations.AddCategory,
                Description = Translations.Name
            };
            input.Show();
            input.Confirmed += delegate
            {
                viewModel.AddAndSetCategory(input.Text);
                Category.ForceRefresh(ProgressBar);
            };
        }

        private void EditCategory_Click(object sender, RoutedEventArgs e)
        {
            Category.IsExpanded = false;
            var input = new XnaInputBox(this)
            {
                Title = Translations.EditCategory,
                Description = Translations.Name,
                Text = viewModel.CategoryName
            };
            input.Show();
            input.Confirmed += delegate
            {
                viewModel.CategoryName = input.Text;
                Category.ForceRefresh(ProgressBar);
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
                MessageBox.Show(Translations.ThisCategoryIncludesOtherProducts,
                    Translations.CannotDelete, MessageBoxButton.OK);
            }
        }

        private void DeleteCategory()
        {
            if (MessageBox.Show(
                String.Format(Translations.AreYouSureYouWantToPermanentlyDeleteThisCategory,
                viewModel.CategoryName),
                Translations.DeleteCategory, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                Save.IsEnabled = false;
                Category.IsExpanded = false;
                Dispatcher.BeginInvoke(() =>
                {
                    viewModel.DeleteCategory();
                    Category.ForceRefresh(ProgressBar);
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
                    viewModel.SaveAndReturn();
                }
            });
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            viewModel.CancelAndReturn();
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            var product = viewModel.Product;
            if (MessageBox.Show(
                String.Format(Translations.AreYouSureYouWantToPermanentlyDeleteThisProduct,
                product.Name),
                Translations.DeleteProduct, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
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
            e.Ignore = (MessageBox.Show(e.Reason, Translations.AreYouSureYouWantToSaveThisProduct,
                MessageBoxButton.OKCancel) == MessageBoxResult.OK);
        }

        private void Categories_ItemClick(object sender, SelectorItemClickEventArgs e)
        {
            Vibration vibration = new VibrationImpl();
            vibration.VibrateOnButtonPress();
        }

        private void Cu_Click(object sender, MouseButtonEventArgs e)
        {
            var learn = new LearningCuAndFpu();
            learn.LearnCu();
        }

        private void Fpu_Click(object sender, MouseButtonEventArgs e)
        {
            var learn = new LearningCuAndFpu();
            learn.LearnFpu();
        }

        private void TranslateApplicationBar()
        {
            Save.Text = Translations.Save;
            this.GetIcon(1).Text = Translations.Cancel;
            this.GetMenuItem(0).Text = Translations.Delete;
        }
    }
}