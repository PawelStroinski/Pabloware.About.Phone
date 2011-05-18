using System.Collections.Generic;
using System;

namespace Dietphone.Models
{
    public class Factory<T> where T : Entity, new()
    {
        private List<T> entities;
        private readonly Factories owner;
        private readonly Storage<T> storage;
        private readonly object entitiesLock = new object();

        public Factory(Factories owner, Storage<T> storage)
        {
            this.owner = owner;
            this.storage = storage;
        }

        public List<T> Entities
        {
            get
            {
                lock (entitiesLock)
                {
                    if (entities == null)
                    {
                        entities = storage.Load();
                        AssignOwner();
                    }
                    return entities;
                }
            }
        }

        public T CreateEntity()
        {
            var entity = new T();
            Entities.Add(entity);
            entity.Owner = owner;
            return entity;
        }

        public void Save()
        {
            if (entities != null)
            {
                storage.Save(Entities);
            }
        }

        private void AssignOwner()
        {
            foreach (var entity in entities)
            {
                entity.Owner = owner;
            }
        }
    }

    public class FactoryCreator
    {
        private readonly Factories owner;
        private readonly StorageCreator storageCreator;

        public FactoryCreator(Factories owner, StorageCreator storageCreator)
        {
            this.owner = owner;
            this.storageCreator = storageCreator;
        }

        public Factory<T> CreateFactory<T>() where T : Entity, new()
        {
            var storage = storageCreator.CreateStorage<T>();
            var factory = new Factory<T>(owner, storage);
            return factory;
        }
    }
}