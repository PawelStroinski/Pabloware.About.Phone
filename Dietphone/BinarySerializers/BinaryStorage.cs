using System.Collections.Generic;
using Dietphone.Models;

namespace Dietphone.BinarySerializers
{
    public abstract class BinaryStorage<T> : BinaryFile<T>, Storage<T> where T : Entity, new()
    {
        public List<T> Load()
        {
            return ReadFile();
        }

        public void Save(List<T> entities)
        {
            WriteFile(entities);
        }
    }

    public class BinaryStorageCreator : StorageCreator
    {
        private readonly BinaryStreamProvider streamProvider;

        public BinaryStorageCreator()
        {
            streamProvider = new BinaryStreamProviderImpl()
            {
                FirstRunDirectory = "firstrun"
            };
        }

        public Storage<T> CreateStorage<T>() where T : Entity, new()
        {
            var builder = new StorageBuilder<BinaryStorage<T>>();
            builder.ProposeStorageForEntity<MealBinaryStorage>();
            builder.ProposeStorageForEntity<MealNameBinaryStorage>();
            builder.ProposeStorageForEntity<ProductBinaryStorage>();
            builder.ProposeStorageForEntity<CategoryBinaryStorage>();
            var storage = builder.RightStorageForEntity;
            ConfigureBinaryFile<T>(storage);
            return storage;
        }

        protected void ConfigureBinaryFile<T>(BinaryFile<T> binaryFile) where T : new()
        {
            binaryFile.StreamProvider = streamProvider;
        }
    }
}
