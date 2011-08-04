namespace Dietphone.BinarySerializers
{
    public sealed class DesktopBinaryStorageCreator : BinaryStorageCreator
    {
        public DesktopBinaryStorageCreator()
            : base(new DesktopBinaryStreamProvider())
        {
        }
    }
}