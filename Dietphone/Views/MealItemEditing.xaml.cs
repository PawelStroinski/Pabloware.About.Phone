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
using Dietphone.Models;
using Dietphone.ViewModels;
using Telerik.Windows.Controls;
using System.Threading;
using Dietphone.Tools;

namespace Dietphone.Views
{
    public partial class MealItemEditing : UserControl
    {
        public MealItemViewModel ViewModel { get; set; }
        public MealItemEditing()
        {
            InitializeComponent();
            var mealItem = new MealItem();
            mealItem.Owner = App.Factories;
            mealItem.ProductId = App.Factories.Products[0].Id;
            mealItem.Value = 50;
            ViewModel = new MealItemViewModel(mealItem);
            DataContext = ViewModel;
        }

        private void ApplicationBarInfo_ButtonClick(object sender, ApplicationBarButtonClickEventArgs e)
        {

        }

        private void Unit_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Unit.IsEnabled)
            {
                Unit.Opacity = 1;
            }
            else
            {
                Unit.Opacity = 0.5;
            }
        }

        private void Picker_PopupOpened(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                Value.Focus();
                if (Value.Text.Length > 0)
                {
                    Value.Select(Value.Text.Length, 0);
                }
            });
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Close()
        {
            if (Value.IsFocused())
            {
                Focus();
                Dispatcher.BeginInvoke(() =>
                {
                    Picker.IsPopupOpen = false;
                });
            }
            else
            {
                Picker.IsPopupOpen = false;
            }
        }
    }
}
