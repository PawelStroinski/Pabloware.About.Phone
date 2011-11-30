using System.IO;
using System.Collections.Generic;
using Dietphone.Models;

namespace Dietphone.BinarySerializers
{
    public sealed class CategoryBinaryStorage : BinaryStorage<Category>
    {
        protected override string FileName
        {
            get
            {
                return string.Format("categories.{0}.db", CultureName);
            }
        }

        protected override byte WritingVersion
        {
            get
            {
                return 1;
            }
        }

        public override void WriteItem(BinaryWriter writer, Category category)
        {
            writer.Write(category.Id);
            writer.WriteString(category.Name);
        }

        public override void ReadItem(BinaryReader reader, Category category)
        {
            category.Id = reader.ReadGuid();
            category.Name = reader.ReadString();
        }
    }
}
