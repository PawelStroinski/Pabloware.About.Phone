using System.IO;

namespace Dietphone.BinarySerializers
{
    public interface BinaryStreamProvider
    {
        Stream GetInputStream(string fileName);

        Stream GetOutputStream(string fileName);
    }
}