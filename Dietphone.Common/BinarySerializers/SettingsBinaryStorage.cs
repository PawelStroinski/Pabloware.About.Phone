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
                return 3;
            }
        }

        public override void WriteItem(BinaryWriter writer, Settings item)
        {
            writer.Write(item.ScoreEnergy);
            writer.Write(item.ScoreProtein);
            writer.Write(item.ScoreDigestibleCarbs);
            writer.Write(item.ScoreFat);
            writer.Write(item.ScoreCu);
            writer.Write(item.ScoreFpu);
            writer.WriteString(item.NextUiCulture);
            writer.WriteString(item.NextProductCulture);
        }

        public override void ReadItem(BinaryReader reader, Settings item)
        {
            item.ScoreEnergy = reader.ReadBoolean();
            item.ScoreProtein = reader.ReadBoolean();
            item.ScoreDigestibleCarbs = reader.ReadBoolean();
            item.ScoreFat = reader.ReadBoolean();
            item.ScoreCu = reader.ReadBoolean();
            item.ScoreFpu = reader.ReadBoolean();
            if (ReadingVersion == 3)
            {
                item.NextUiCulture = reader.ReadString();
                item.NextProductCulture = reader.ReadString();
            }
        }
    }
}
