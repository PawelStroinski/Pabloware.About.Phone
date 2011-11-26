using System;
using Dietphone.Tools;
using System.Globalization;
using Dietphone.Views;

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
                return Translations.Older;
            }
            if (Date.IsYesterday())
            {
                return Translations.Yesterday;
            }
            if (Date.IsToday())
            {
                return Translations.Today;
            }
            return Date.ToShortDateInAlternativeFormat();
        }
    }
}
