using System;
using System.Windows;
using System.Windows.Controls;
using Dietphone.ViewModels;
using Dietphone.Tools;

namespace Dietphone.Views
{
    public partial class MealItemEditing : UserControl
    {
        public MealItemEditingViewModel ViewModel { get; private set; }

        public MealItemEditing()
        {
            InitializeComponent();
            Delete = Picker.ApplicationBarInfo.Buttons[2];
            ViewModel = new MealItemEditingViewModel();
            ViewModel.Showing += delegate
            {
                DataContext = ViewModel.MealItem;
                Delete.IsEnabled = ViewModel.CanDelete;
                Picker.IsPopupOpen = true;
            };
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
            ViewModel.Confirm();
            Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            ViewModel.Cancel();
            Close();
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            ViewModel.Delete();
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
