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
}