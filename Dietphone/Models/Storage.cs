using System.Collections.Generic;
using System;

namespace Dietphone.Models
{
    public interface Storage<T> where T : Entity, new()
    {
        List<T> Load();
        void Save(List<T> entities);
    }

    public interface StorageCreator
    {
        Storage<T> CreateStorage<T>() where T : Entity, new();
    }

    public class StorageBuilder<TBaseStorageForEntity> where TBaseStorageForEntity : class
    {
        private TBaseStorageForEntity rightStorageForEntity;

        public TBaseStorageForEntity RightStorageForEntity
        {
            get
            {
                if (rightStorageForEntity == null)
                {
                    throw new NotImplementedException(typeof(TBaseStorageForEntity).ToString());
                }
                return rightStorageForEntity;
            }
        }

        public void ProposeStorageForEntity<TStorage>() where TStorage : new()
        {
            var alreadyFound = rightStorageForEntity != null;
            if (alreadyFound)
            {
                return;
            }
            if (typeof(TStorage).IsSubclassOf(typeof(TBaseStorageForEntity)))
            {
                rightStorageForEntity = new TStorage() as TBaseStorageForEntity;
            }
        }
    }
}