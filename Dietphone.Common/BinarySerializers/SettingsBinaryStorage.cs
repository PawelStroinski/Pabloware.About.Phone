using Dietphone.Models;
using System.IO;

namespace Dietphone.BinarySerializers
{
    public class SettingsBinaryStorage : BinaryStorage<Settings>
    {

        protected override string FileName
        {
            get
            {
                return "settings.db";
            }
        }

        protected override byte WritingVersion
        {
            get
            {
                return 2;
            }
        }

        public override void WriteItem(BinaryWriter writer, Settings item)
        {
            writer.Write(item.CalculateEnergy);
            writer.Write(item.CalculateProteinInGrams);
            writer.Write(item.CalculateDigestibleCarbsInGrams);
            writer.Write(item.CalculateFatInGrams);
            writer.Write(item.CalculateCu);
            writer.Write(item.CalculateFpu);
            writer.Write(item.FirstRun);
        }

        public override void ReadItem(BinaryReader reader, Settings item)
        {
            item.CalculateEnergy = reader.ReadBoolean();
            item.CalculateProteinInGrams = reader.ReadBoolean();
            item.CalculateDigestibleCarbsInGrams = reader.ReadBoolean();
            item.CalculateFatInGrams = reader.ReadBoolean();
            item.CalculateCu = reader.ReadBoolean();
            item.CalculateFpu = reader.ReadBoolean();
            if (ReadingVersion == 2)
            {
                item.FirstRun = reader.ReadBoolean();
            }
        }
    }
}
