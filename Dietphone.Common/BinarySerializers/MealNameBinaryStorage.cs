using System.IO;
using System.Collections.Generic;
using Dietphone.Models;

namespace Dietphone.BinarySerializers
{
    public sealed class MealNameBinaryStorage : BinaryStorage<MealName>
    {
        protected override string FileName
        {
            get
            {
                return "mealnames.db";
            }
        }

        protected override byte WritingVersion
        {
            get
            {
                return 1;
            }
        }

        public override void WriteItem(BinaryWriter writer, MealName mealName)
        {
            writer.Write(mealName.Id);
            writer.WriteString(mealName.Name);
        }

        public override void ReadItem(BinaryReader reader, MealName mealName)
        {
            mealName.Id = reader.ReadGuid();
            mealName.Name = reader.ReadString();
        }
    }
}
