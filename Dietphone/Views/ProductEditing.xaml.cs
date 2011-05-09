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
using Coding4Fun.Phone.Controls;

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

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Categories.IsExpanded = false;
            var addCategoryInput = new InputPrompt
            {
                Title = "DODAJ KATEGORIĘ",
                Message = "Nazwa",
                IsCancelVisible = true
            };
            addCategoryInput.Completed +=
                new EventHandler<PopUpEventArgs<string, PopUpResult>>(addCategoryInput_Completed);
            addCategoryInput.Show();
        }

        private void addCategoryInput_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            if (e.PopUpResult == PopUpResult.Ok && e.Result != "")
            {
                viewModel.AddCategory(e.Result);
            }
        }
    }
}