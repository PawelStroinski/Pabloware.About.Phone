using System;
using System.IO;
using System.Windows;
using Dietphone.Tools;

namespace Dietphone.BinarySerializers
{
    public sealed class PhoneBinaryStreamProvider : BinaryStreamProvider
    {
        private const string FIRST_RUN_DIRECTORY = "firstrun";

        public Stream GetInputStream(string fileName)
        {
            var file = new IsolatedFile(fileName);
            if (file.Exists)
            {
                return file.GetReadingStream();
            }
            else
            {
                var relativePath = Path.Combine(FIRST_RUN_DIRECTORY, fileName);
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