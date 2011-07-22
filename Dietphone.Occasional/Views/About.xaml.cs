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
using Telerik.Windows.Controls;
using Dietphone.ViewModels;

namespace Dietphone.Views
{
    public partial class About : PhoneApplicationPage
    {
        private AboutViewModel viewModel = new AboutViewModel();

        public About()
        {
            InitializeComponent();
            DataContext = viewModel;
            InteractionEffectManager.AllowedTypes.Add(typeof(TextBlock));
        }

        private void Web_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            viewModel.OpenWeb();
        }

        private void Review_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            viewModel.OpenReview();
        }

        private void Feedback_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            viewModel.OpenFeedback();
        }
    }
}