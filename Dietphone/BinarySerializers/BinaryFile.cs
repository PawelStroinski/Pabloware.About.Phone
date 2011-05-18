using System;
using System.Windows;
using System.IO;
using System.Collections.Generic;
using Dietphone.Tools;

namespace Dietphone.BinarySerializers
{
    public abstract class BinaryFile<T> : BinarySerializer<T> where T : new()
    {
        protected abstract string FileName { get; }
        protected abstract byte WritingVersion { get; }
        protected Byte ReadingVersion { get; private set; }
        public string FirstRunDirectory { get; set; }

        public abstract void WriteItem(BinaryWriter writer, T item);

        public abstract void ReadItem(BinaryReader reader, T item);

        protected List<T> ReadFile()
        {
            using (var input = GetInputStream())
            {
                using (var reader = new BinaryReader(input))
                {
                    ReadingVersion = reader.ReadByte();
                    return reader.ReadList<T>(this);
                }
            }
        }

        protected void WriteFile(List<T> items)
        {
            using (var output = GetOutputStream())
            {
                using (var writer = new BinaryWriter(output))
                {
                    writer.Write(WritingVersion);
                    writer.WriteList<T>(items, this);
                }
            }
        }

        protected Stream GetInputStream()
        {
            var file = new IsolatedFile(FileName);
            if (file.Exists)
            {
                return file.GetReadingStream();
            }
            else
            {
                var relativePath = Path.Combine(FirstRunDirectory, FileName);
                var resource = Application.GetResourceStream(new Uri(relativePath, UriKind.Relative));
                return resource.Stream;
            }
        }

        protected Stream GetOutputStream()
        {
            var file = new IsolatedFile(FileName);
            return file.GetWritingStream();
        }
    }
}