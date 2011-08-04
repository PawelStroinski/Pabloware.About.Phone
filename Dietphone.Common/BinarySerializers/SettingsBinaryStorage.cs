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
                return 1;
            }
        }

        public override void WriteItem(BinaryWriter writer, Settings item)
        {
            writer.Write(item.ShowEnergy);
            writer.Write(item.ShowProteinInGrams);
            writer.Write(item.ShowDigestibleCarbsInGrams);
            writer.Write(item.ShowFatInGrams);
            writer.Write(item.ShowCu);
            writer.Write(item.ShowFpu);
        }

        public override void ReadItem(BinaryReader reader, Settings item)
        {
            item.ShowEnergy = reader.ReadBoolean();
            item.ShowProteinInGrams = reader.ReadBoolean();
            item.ShowDigestibleCarbsInGrams = reader.ReadBoolean();
            item.ShowFatInGrams = reader.ReadBoolean();
            item.ShowCu = reader.ReadBoolean();
            item.ShowFpu = reader.ReadBoolean();
        }
    }
}
