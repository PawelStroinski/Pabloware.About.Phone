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
                return 2;
            }
        }

        public override void WriteItem(BinaryWriter writer, MealName mealName)
        {
            writer.Write(mealName.Id);
            writer.Write((byte)mealName.Kind);
            if (mealName.Kind == MealNameKind.Custom)
            {
                writer.WriteString(mealName.Name);
            }
        }

        public override void ReadItem(BinaryReader reader, MealName mealName)
        {
            mealName.Id = reader.ReadGuid();
            if (ReadingVersion == 1)
            {
                mealName.Name = reader.ReadString();
            }
            else
            {
                mealName.Kind = (MealNameKind)reader.ReadByte();
                if (mealName.Kind == MealNameKind.Custom)
                {
                    mealName.Name = reader.ReadString();
                }
            }
        }
    }
}
