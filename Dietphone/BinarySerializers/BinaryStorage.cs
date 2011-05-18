using System;
using System.IO;
using System.Collections.Generic;
using Dietphone.Models;
using System.Windows;

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
        public Storage<T> CreateStorage<T>() where T : Entity, new()
        {
            var builder = new StorageBuilder<BinaryStorage<T>>();
            builder.ProposeStorageForEntity<CategoryBinaryStorage>();
            builder.ProposeStorageForEntity<ProductBinaryStorage>();
            var storage = builder.RightStorageForEntity;
            ConfigureBinaryFile<T>(storage);
            return storage;
        }

        protected void ConfigureBinaryFile<T>(BinaryFile<T> storage) where T : new()
        {
            storage.FirstRunDirectory = "firstrun";
        }
    }
}
