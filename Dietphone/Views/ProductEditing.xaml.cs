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

namespace Dietphone.Views
{
    public partial class ProductEditing : PhoneApplicationPage
    {
        private ProductEditingViewModel viewModel;
        private XnaInputBox addCategoryBox;

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

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Categories.IsExpanded = false;
            addCategoryBox = new XnaInputBox(this);
            addCategoryBox.Title = "DODAJ KATEGORIĘ";
            addCategoryBox.Description = "Nazwa";
            addCategoryBox.Ok += new EventHandler(addCategoryBox_Ok);
            addCategoryBox.Show();
        }

        private void addCategoryBox_Ok(object sender, EventArgs e)
        {
            viewModel.AddCategory(addCategoryBox.Text);
        }
    }
}