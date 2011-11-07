// Na podstawie WindowsPhone7UXGuide
using System.Windows;

namespace Dietphone.Views
{
    public class MultiLineItem : DependencyObject
    {
        public string Line1
        {
            get
            {
                return (string)GetValue(Line1Property);
            }
            set
            {
                SetValue(Line1Property, value);
            }
        }

        public string Line2
        {
            get
            {
                return (string)GetValue(Line2Property);
            }
            set
            {
                SetValue(Line2Property, value);
            }
        }

        public static readonly DependencyProperty Line1Property = DependencyProperty.Register(
            "Line1", typeof(string), typeof(MultiLineItem), null);

        public static readonly DependencyProperty Line2Property = DependencyProperty.Register(
            "Line2", typeof(string), typeof(MultiLineItem), null);
    }
}
