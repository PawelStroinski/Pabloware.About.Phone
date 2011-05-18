using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Dietphone.Tools
{
    public static class UIExtensionMethods
    {
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

        public static ApplicationBarIconButton GetIcon(this PhoneApplicationPage page, int whichIcon)
        {
            var appBar = page.ApplicationBar;
            var icons = appBar.Buttons;
            return icons[whichIcon] as ApplicationBarIconButton;
        }
    }
}
