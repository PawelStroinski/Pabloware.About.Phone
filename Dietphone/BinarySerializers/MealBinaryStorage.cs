using System.IO;
using System.Collections.Generic;
using Dietphone.Models;

namespace Dietphone.BinarySerializers
{
    public sealed class MealBinaryStorage : BinaryStorage<Meal>
    {
        private readonly MealItemBinarySerializer itemSerializer;

        public MealBinaryStorage()
        {
            itemSerializer = new MealItemBinarySerializer();
        }

        protected override string FileName
        {
            get
            {
                return "meals.db";
            }
        }

        protected override byte WritingVersion
        {
            get
            {
                return 1;
            }
        }

        public override void WriteItem(BinaryWriter writer, Meal meal)
        {
            writer.Write(meal.Id);
            writer.Write(meal.DateTime);
            writer.Write(meal.NameId);
            writer.WriteString(meal.Note);
            writer.WriteList(meal.Items, itemSerializer);
        }

        public override void ReadItem(BinaryReader reader, Meal meal)
        {
            meal.Id = reader.ReadGuid();
            meal.DateTime = reader.ReadDateTime();
            meal.NameId = reader.ReadGuid();
            meal.Note = reader.ReadString();
            var items = reader.ReadList(itemSerializer);
            meal.InitializeItems(items);
        }

        private sealed class MealItemBinarySerializer : BinarySerializer<MealItem>
        {
            public void WriteItem(BinaryWriter writer, MealItem item)
            {
                writer.Write(item.Value);
                writer.Write((byte)item.Unit);
                writer.Write(item.ProductId);
            }

            public void ReadItem(BinaryReader reader, MealItem item)
            {
                item.Value = reader.ReadSingle();
                item.Unit = (Unit)reader.ReadByte();
                item.ProductId = reader.ReadGuid();
            }
        }
    }
}
