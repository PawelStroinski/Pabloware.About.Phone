using Dietphone.Models;

namespace Dietphone.BinarySerializers
{
    public abstract class BinaryStorageCreator : StorageCreator
    {
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
            var storage = builder.RightStorageForEntity;
            storage.StreamProvider = streamProvider;
            return storage;
        }
    }
}