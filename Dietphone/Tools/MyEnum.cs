// Na podstawie http://stackoverflow.com/questions/105372/c-how-to-enumerate-an-enum
using System.Collections.Generic;
using System.Reflection;

namespace Dietphone.Tools
{
    public static class MyEnum
    {
        public static IEnumerable<T> GetValues<T>()
        {
            var enumerations = new List<T>();
            var enumType = typeof(T);
            var enumFields = enumType.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var fieldInfo in enumFields)
            {
                enumerations.Add((T)fieldInfo.GetValue(enumType));
            }
            return enumerations;
        }
    }
}