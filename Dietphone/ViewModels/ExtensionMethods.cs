// Metoda CopyFromAny inspirowana metodą CopyTo z http://stackoverflow.com/questions/78536/cloning-objects-in-c
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
using System.Collections.Generic;
using Microsoft.Phone.Shell;

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

        public static List<T> GetItemsCopy<T>(this List<T> source) where T : class, new()
        {
            var target = new List<T>();
            foreach (var sourceItem in source)
            {
                var targetItem = sourceItem.GetCopy();
                target.Add(targetItem);
            }
            return target;
        }

        public static T GetCopy<T>(this T source) where T : class, new()
        {
            var target = new T();
            target.CopyFrom(source);
            return target;
        }

        public static void CopyFrom<T>(this T target, T source) where T : class
        {
            var type = target.GetType();
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var getMethod = property.GetGetMethod();
                var setMethod = property.GetSetMethod();
                if (getMethod != null && setMethod != null)
                {
                    var value = getMethod.Invoke(source, null);
                    var parameters = new object[] { value };
                    setMethod.Invoke(target, parameters);
                }
            }
        }

        public static void CopyFromAny(this object target, object source)
        {
            var targetType = target.GetType();
            var sourceType = source.GetType();
            var targetProperties = targetType.GetProperties();
            var sourceProperties = sourceType.GetProperties();
            foreach (var sourceProperty in sourceProperties)
            {
                foreach (var targetProperty in targetProperties)
                {
                    if (targetProperty.Name != sourceProperty.Name)
                    {
                        continue;
                    }
                    var setMethod = targetProperty.GetSetMethod();
                    var getMethod = sourceProperty.GetGetMethod();
                    if (setMethod != null && getMethod != null)
                    {
                        var value = getMethod.Invoke(source, null);
                        var parameters = new object[] { value };
                        setMethod.Invoke(target, parameters);
                    }
                }
            }
        }

        public static ApplicationBarIconButton GetIcon(this PhoneApplicationPage page, int whichIcon)
        {
            var appBar = page.ApplicationBar;
            var icons = appBar.Buttons;
            return icons[whichIcon] as ApplicationBarIconButton;
        }
    }
}
