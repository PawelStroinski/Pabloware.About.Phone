using System;

namespace Dietphone.Models
{
    public class Entity
    {
        public void SetOwner(Factories value)
        {
            var alreadyAssigned = Owner != null;
            if (alreadyAssigned)
            {
                throw new InvalidOperationException("Owner can only be assigned once.");
            }
            if (value == null)
            {
                throw new NullReferenceException("Owner");
            }
            Owner = value;
            OnOwnerAssigned();
        }

        protected Factories Owner { get; private set; }

        protected Finder Finder
        {
            get
            {
                return Owner.Finder;
            }
        }

        protected DefaultEntities DefaultEntities
        {
            get
            {
                return Owner.DefaultEntities;
            }
        }

        protected virtual void OnOwnerAssigned()
        {
        }
    }

    public class EntityWithId : Entity, HasId
    {
        public Guid Id { get; set; }
    }
}