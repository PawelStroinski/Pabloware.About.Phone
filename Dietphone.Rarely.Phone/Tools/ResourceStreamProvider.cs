using System.IO;

namespace Dietphone.Tools
{
    public interface ResourceStreamProvider
    {
        Stream GetResourceStream(string uri);
    }
}
