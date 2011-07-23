using System;
using System.IO;
using System.Windows;

namespace Dietphone.Tools
{
    public class PhoneResourceStreamProvider : ResourceStreamProvider
    {
        public Stream GetResourceStream(string uri)
        {
            var relative = new Uri(uri, UriKind.Relative);
            var resource = Application.GetResourceStream(relative);
            return resource.Stream;
        }
    }
}
