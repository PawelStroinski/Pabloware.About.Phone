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

namespace Dietphone.Views
{
    public partial class ProductEditing : PhoneApplicationPage
    {
        private ProductEditingViewModel viewModel;

        public ProductEditing()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var navigator = new NavigatorImpl(NavigationService, NavigationContext);
            viewModel = new ProductEditingViewModel(App.Factories, navigator);
            DataContext = viewModel;
        }

        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            Categories.IsExpanded = false;
            var input = new XnaInputBox(this)
            {
                Title = "DODAJ KATEGORIĘ",
                Description = "Nazwa"
            };
            input.Confirmed += delegate { viewModel.AddAndSetCategory(input.Text); };
            input.Show();
        }

        private void RenameCategory_Click(object sender, RoutedEventArgs e)
        {
            Categories.IsExpanded = false;
            var input = new XnaInputBox(this)
            {
                Title = "EDYTUJ KATEGORIĘ",
                Description = "Nazwa",
                Text = viewModel.CategoryName
            };
            input.Confirmed += delegate
            {
                viewModel.CategoryName = input.Text;
                Categories.ForceRefresh(ProgressBar);
            };
            input.Show();
        }
    }
}