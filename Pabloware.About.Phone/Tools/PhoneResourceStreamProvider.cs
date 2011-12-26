using System;
using System.IO;
using System.Windows;

namespace Pabloware.About.Tools
{
    internal class PhoneResourceStreamProvider : ResourceStreamProvider
    {
        public Stream GetResourceStream(string uri)
        {
            var relative = new Uri(uri, UriKind.Relative);
            var resource = Application.GetResourceStream(relative);
            return resource.Stream;
        }
    }
}
