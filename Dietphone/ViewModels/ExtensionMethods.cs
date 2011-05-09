using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;
using Microsoft.Phone.Controls;

namespace Dietphone.ViewModels
{
    public static class ExtensionMethods
    {
        public static bool ContainsIgnoringCase(this string source, string toCheck)
        {
            return source.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static bool EqualsIgnoringCase(this string source, string toCheck)
        {
            return source.Equals(toCheck, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsFocused(this Control control)
        {
            return FocusManager.GetFocusedElement() == control;
        }

        public static void UniversalGroupPickerItemTap(this RadJumpList list, GroupPickerItemTapEventArgs e)
        {
            var groups = list.Groups;
            foreach (var group in groups)
            {
                if (object.Equals(e.DataItem, group.Key))
                {
                    e.DataItemToNavigate = group;
                    return;
                }
            }
        }

        public static void ForceRefresh(this RadListPicker picker, PerformanceProgressBar progressBar)
        {
            progressBar.IsIndeterminate = true;
            picker.Dispatcher.BeginInvoke(() =>
            {
                picker.IsEnabled = false;
                var items = picker.ItemsSource;
                var item = picker.SelectedItem;
                picker.ItemsSource = null;
                picker.ItemsSource = items;
                picker.SelectedItem = item;
                picker.IsEnabled = true;
                progressBar.IsIndeterminate = false;
            });
        }
    }
}
