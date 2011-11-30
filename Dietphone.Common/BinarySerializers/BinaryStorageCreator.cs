using Dietphone.Models;

namespace Dietphone.BinarySerializers
{
    public sealed class BinaryStorageCreator : StorageCreator
    {
        public string CultureName { private get; set; }
        private readonly BinaryStreamProvider streamProvider;

        public BinaryStorageCreator(BinaryStreamProvider streamProvider)
        {
            this.streamProvider = streamProvider;
        }

        public Storage<T> CreateStorage<T>() where T : Entity, new()
        {
            var builder = new StorageBuilder<BinaryStorage<T>>();
            builder.ProposeStorageForEntity<MealBinaryStorage>();
            builder.ProposeStorageForEntity<MealNameBinaryStorage>();
            builder.ProposeStorageForEntity<ProductBinaryStorage>();
            builder.ProposeStorageForEntity<CategoryBinaryStorage>();
            builder.ProposeStorageForEntity<SettingsBinaryStorage>();
            var storage = builder.RightStorageForEntity;
            storage.StreamProvider = streamProvider;
            storage.CultureName = CultureName;
            return storage;
        }
    }
}