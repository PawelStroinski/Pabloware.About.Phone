namespace Dietphone.Models
{
    public interface ExporterAndImporter
    {
        string Export();
        void Import(string data);
    }
}
