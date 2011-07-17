namespace Dietphone.BinarySerializers
{
    public sealed class PhoneBinaryStorageCreator : BinaryStorageCreator
    {
        public PhoneBinaryStorageCreator()
            : base(new PhoneBinaryStreamProvider())
        {
        }
    }
}