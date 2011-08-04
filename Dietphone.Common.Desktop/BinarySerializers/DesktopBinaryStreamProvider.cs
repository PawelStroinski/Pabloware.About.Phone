using System.IO;

namespace Dietphone.BinarySerializers
{
    public sealed class DesktopBinaryStreamProvider : BinaryStreamProvider
    {
        public const string DIRECTORY = @"c:\temp\dietphone";

        public Stream GetInputStream(string fileName)
        {
            var path = Path.Combine(DIRECTORY, fileName);
            return new FileStream(path, FileMode.Open);
        }

        public Stream GetOutputStream(string fileName)
        {
            var path = Path.Combine(DIRECTORY, fileName);
            return new FileStream(path, FileMode.Truncate);
        }
    }
}