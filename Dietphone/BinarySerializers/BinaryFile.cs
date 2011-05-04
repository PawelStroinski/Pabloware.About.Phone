using System;
using System.Windows;
using System.IO;
using System.Collections.Generic;

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

        protected Stream GetInputStream()
        {
            var relativePath = FirstRunDirectory + FileName;
            var resource = Application.GetResourceStream(new Uri(relativePath, UriKind.Relative));
            return resource.Stream;
        }
    }
}