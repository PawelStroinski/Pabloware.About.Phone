// Metoda CopyFromAny inspirowana metodą CopyTo z http://stackoverflow.com/questions/78536/cloning-objects-in-c
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Dietphone.Tools
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
            var type = typeof(T);
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

        public static string ToStringOrEmpty(this float value)
        {
            if (value == 0)
            {
                return String.Empty;
            }
            else
            {
                return value.ToString();
            }
        }

        public static string ToStringOrEmpty(this short value)
        {
            if (value == 0)
            {
                return String.Empty;
            }
            else
            {
                return value.ToString();
            }
        }

        public static string JoinOptionalSentences(this IEnumerable<string> optionalSentences)
        {
            var sentences = optionalSentences.
                Where(optionalSentence => !string.IsNullOrEmpty(optionalSentence));
            return string.Join(" ", sentences.ToArray());
        }

        public static string ToShortDateInAlternativeFormat(this DateTime date)
        {
            var culture = CultureInfo.CurrentCulture;
            var format = culture.GetShortDateAlternativeFormat();
            return date.ToString(format);
        }

        public static bool IsToday(this DateTime time)
        {
            return DateTime.Today == time.Date;
        }

        public static bool IsYesterday(this DateTime time)
        {
            return DateTime.Today - time.Date == TimeSpan.FromDays(1);
        }

        public static string GetShortDateAlternativeFormat(this CultureInfo culture)
        {
            if (culture.IsPolish())
            {
                return "dd.MM.yyyy";
            }
            var defaultFormat = culture.DateTimeFormat;
            return defaultFormat.ShortDatePattern;
        }

        public static bool IsPolish(this CultureInfo culture)
        {
            return culture.TwoLetterISOLanguageName == "pl";
        }

        public static void SetNullStringPropertiesToEmpty(this object target)
        {
            var type = target.GetType();
            var properties = type.GetProperties();
            var emptyString = new object[] { string.Empty };
            var stringType = typeof(string);
            foreach (var property in properties)
            {
                if (property.PropertyType == stringType)
                {
                    var getMethod = property.GetGetMethod();
                    var setMethod = property.GetSetMethod();
                    if (getMethod != null && setMethod != null)
                    {
                        var value = getMethod.Invoke(target, null);
                        if (value == null)
                        {
                            setMethod.Invoke(target, emptyString);
                        }
                    }
                }
            }
        }

        public static T GetNextItemToSelectWhenDeleteSelected<T>(this IList<T> items, T selected)
        {
            if (items.Count < 2)
            {
                throw new InvalidOperationException("List must have at least two items.");
            }
            if (!items.Contains(selected))
            {
                throw new InvalidOperationException("Selected item not found.");
            }
            return items.GetNextItemToSelectWhenDeleteSelectedCore(selected);
        }

        private static T GetNextItemToSelectWhenDeleteSelectedCore<T>(this IList<T> items, T selected)
        {
            var indexOfSelected = items.IndexOf(selected);
            var indexOfLastItem = items.Count - 1;
            int indexOfNextSelected;
            if (indexOfSelected == indexOfLastItem)
            {
                indexOfNextSelected = indexOfLastItem - 1;
            }
            else
            {
                indexOfNextSelected = indexOfSelected + 1;
            }
            return items[indexOfNextSelected];
        }

        public static string Serialize(this object source, string defaultNamespaceUri)
        {
            var sourceType = source.GetType();
            var serializer = new XmlSerializer(sourceType, defaultNamespaceUri);
            var builder = new StringBuilder();
            var writer = XmlWriter.Create(builder);
            var defaultNamespace = new XmlSerializerNamespaces();
            defaultNamespace.Add(string.Empty, defaultNamespaceUri);
            serializer.Serialize(writer, source, defaultNamespace);
            return builder.ToString();
        }

        public static T Deserialize<T>(this string source, string defaultNamespaceUri)
        {
            var serializer = new XmlSerializer(typeof(T), defaultNamespaceUri);
            var reader = new StringReader(source);
            return (T)serializer.Deserialize(reader);
        }

        public static bool IsValidEmail(this string source)
        {
            return source.Contains('@') && source.Contains('.');
        }
    }
}
