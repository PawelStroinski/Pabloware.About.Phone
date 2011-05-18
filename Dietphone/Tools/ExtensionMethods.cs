// Metoda CopyFromAny inspirowana metodą CopyTo z http://stackoverflow.com/questions/78536/cloning-objects-in-c
using System;
using System.Collections.Generic;

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

        public static string ContactOptionalSentences(this string[] optionalSentences)
        {
            String result = string.Empty;
            foreach (var optionalSentence in optionalSentences)
            {
                if (!string.IsNullOrEmpty(optionalSentence))
                {
                    result += optionalSentence + " ";
                }
            }
            return result;
        }
    }
}
