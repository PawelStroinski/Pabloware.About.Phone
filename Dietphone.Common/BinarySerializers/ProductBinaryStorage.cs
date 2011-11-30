using System.IO;
using System.Collections.Generic;
using Dietphone.Models;

namespace Dietphone.BinarySerializers
{
    public sealed class ProductBinaryStorage : BinaryStorage<Product>
    {
        protected override string FileName
        {
            get
            {
                return string.Format("products.{0}.db", CultureName);
            }
        }

        protected override byte WritingVersion
        {
            get
            {
                return 2;
            }
        }

        public override void WriteItem(BinaryWriter writer, Product product)
        {
            writer.Write(product.Id);
            writer.WriteObfuscated(product.Name);
            writer.Write(product.CategoryId);
            writer.Write(product.ServingSizeValue);
            writer.Write((byte)product.ServingSizeUnit);
            writer.WriteObfuscated(product.ServingSizeDescription);
            writer.Write(product.EnergyPer100g);
            writer.Write(product.EnergyPerServing);
            writer.Write(product.ProteinPer100g);
            writer.Write(product.ProteinPerServing);
            writer.Write(product.FatPer100g);
            writer.Write(product.FatPerServing);
            writer.Write(product.CarbsTotalPer100g);
            writer.Write(product.CarbsTotalPerServing);
            writer.Write(product.FiberPer100g);
            writer.Write(product.FiberPerServing);
            writer.Write(product.AddedByUser);
        }

        public override void ReadItem(BinaryReader reader, Product product)
        {
            product.Id = reader.ReadGuid();
            product.Name = reader.ReadObfuscated();
            product.CategoryId = reader.ReadGuid();
            product.ServingSizeValue = reader.ReadSingle();
            product.ServingSizeUnit = (Unit)reader.ReadByte();
            product.ServingSizeDescription = reader.ReadObfuscated();
            product.EnergyPer100g = reader.ReadInt16();
            product.EnergyPerServing = reader.ReadInt16();
            product.ProteinPer100g = reader.ReadSingle();
            product.ProteinPerServing = reader.ReadSingle();
            product.FatPer100g = reader.ReadSingle();
            product.FatPerServing = reader.ReadSingle();
            product.CarbsTotalPer100g = reader.ReadSingle();
            product.CarbsTotalPerServing = reader.ReadSingle();
            product.FiberPer100g = reader.ReadSingle();
            product.FiberPerServing = reader.ReadSingle();
            if (ReadingVersion == 2)
            {
                product.AddedByUser = reader.ReadBoolean();
            }
        }
    }
}