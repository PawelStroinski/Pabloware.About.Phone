namespace Dietphone.Models
{
    public interface ExportAndImport
    {
        string Export();
        void Import(string data);
    }
}
