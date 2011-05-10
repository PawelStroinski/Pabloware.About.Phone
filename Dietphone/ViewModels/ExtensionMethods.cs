// Metody CopyTo* inspirowane metodą CopyTo z http://stackoverflow.com/questions/78536/cloning-objects-in-c
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

        public static void CopyToSameType<T>(this T source, T target) where T : class
        {
            var type = source.GetType();
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

        public static void CopyToAnyType(this object source, object target)
        {
            var sourceType = source.GetType();
            var targetType = target.GetType();
            var sourceProperties = sourceType.GetProperties();
            var targetProperties = targetType.GetProperties();
            foreach (var sourceProperty in sourceProperties)
            {
                foreach (var targetProperty in targetProperties)
                {
                    if (targetProperty.Name != sourceProperty.Name)
                    {
                        continue;
                    }
                    var getMethod = sourceProperty.GetGetMethod();
                    var setMethod = targetProperty.GetSetMethod();
                    if (getMethod != null && setMethod != null)
                    {
                        var value = getMethod.Invoke(source, null);
                        var parameters = new object[] { value };
                        setMethod.Invoke(target, parameters);
                    }
                }
            }
        }
    }
}
