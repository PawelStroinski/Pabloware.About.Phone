using System;
using Dietphone.Tools;
using System.Globalization;

namespace Dietphone.ViewModels
{
    public class DateViewModel : IComparable
    {
        public DateTime Date { private set; get; }
        public bool IsGroupOfOlder { private set; get; }

        private DateViewModel()
        {
        }

        public static DateViewModel CreateNormalDate(DateTime date)
        {
            return new DateViewModel()
            {
                Date = date
            };
        }

        public static DateViewModel CreateGroupOfOlder()
        {
            return new DateViewModel()
            {
                Date = DateTime.MinValue,
                IsGroupOfOlder = true
            };
        }

        public int CompareTo(object obj)
        {
            var another = obj as DateViewModel;
            if (another == null)
            {
                throw new ArgumentException("Object is not DateViewModel");
            }
            else
            {
                var ascending = DateTime.Compare(Date, another.Date);
                return -ascending;
            }
        }

        public override string ToString()
        {
            if (IsGroupOfOlder)
            {
                return "starsze";
            }
            if (Date.IsYesterday())
            {
                return "wczoraj";
            }
            if (Date.IsToday())
            {
                return "dziś";
            }
            return Date.ToShortDateInAlternativeFormat();
        }
    }
}
