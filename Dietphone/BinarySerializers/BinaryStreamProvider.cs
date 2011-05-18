using System;
using System.Windows;
using System.IO;
using Dietphone.Tools;

namespace Dietphone.BinarySerializers
{
    public interface BinaryStreamProvider
    {
        Stream GetInputStream(string fileName);
        Stream GetOutputStream(string fileName);
    }

    public sealed class BinaryStreamProviderImpl : BinaryStreamProvider
    {
        public string FirstRunDirectory { get; set; }

        public Stream GetInputStream(string fileName)
        {
            var file = new IsolatedFile(fileName);
            if (file.Exists)
            {
                return file.GetReadingStream();
            }
            else
            {
                var relativePath = Path.Combine(FirstRunDirectory, fileName);
                var resource = Application.GetResourceStream(new Uri(relativePath, UriKind.Relative));
                return resource.Stream;
            }
        }

        public Stream GetOutputStream(string fileName)
        {
            var file = new IsolatedFile(fileName);
            return file.GetWritingStream();
        }
    }
}
