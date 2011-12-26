using System.Windows.Navigation;
using System.Collections.Generic;

namespace Pabloware.About.ViewModels
{
    internal class ComingToAbout
    {
        public AboutDto Dto { get; private set; }
        private IDictionary<string, string> queryString;

        public ComingToAbout(NavigationContext context)
        {
            Dto = new AboutDto();
            queryString = context.QueryString;
            DeserializeFromQueryString();
        }

        private void DeserializeFromQueryString()
        {
            var type = typeof(AboutDto);
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var setMethod = property.GetSetMethod();
                if (setMethod != null)
                {
                    var value = string.Empty;
                    queryString.TryGetValue(property.Name, out value);
                    var parameters = new object[] { value };
                    setMethod.Invoke(Dto, parameters);
                }
            }
        }
    }
}
