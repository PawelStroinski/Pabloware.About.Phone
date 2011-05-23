using System;

namespace Dietphone.Models
{
    public class Entity
    {
        private Factories owner;

        public Factories Owner
        {
            protected get
            {
                return owner;
            }
            set
            {
                var alreadyAssigned = owner != null;
                if (alreadyAssigned)
                {
                    throw new InvalidOperationException("Owner can only be assigned once.");
                }
                if (value == null)
                {
                    throw new NullReferenceException("Owner");
                }
                owner = value;
            }
        }

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
    }

    public class EntityWithId : Entity
    {
        public Guid Id { get; set; }
    }
}