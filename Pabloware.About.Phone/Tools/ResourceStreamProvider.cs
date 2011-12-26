using System.IO;

namespace Pabloware.About.Tools
{
    internal interface ResourceStreamProvider
    {
        Stream GetResourceStream(string uri);
    }
}
