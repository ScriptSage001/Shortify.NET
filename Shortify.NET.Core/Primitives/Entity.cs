namespace Shortify.NET.Core.Primitives
{
    /// <summary>
    /// Base Entity Class for Domain Entities
    /// </summary>
    public abstract class Entity : IEquatable<Entity>
    {
        /// <summary>
        /// Constructor to initialize 
        /// the base properties of any entity
        /// </summary>
        /// <param name="id"></param>
        protected Entity(Guid id)
        {
            Id = id;
        }

        /// <summary>
        /// Identifier Property for the Entities
        /// </summary>
        public Guid Id { get; private init; }

        public bool Equals(Entity? other)
        {
            if (other is null ||
                other.GetType() != GetType())
            {
                return false;
            }

            return Id == other.Id;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null ||
                obj.GetType() != GetType() ||
                obj is not Entity entity)
            {
                return false;
            }

            return entity.Id == Id;
        }

        public static bool operator ==(Entity? first, Entity? second)
        {
            return first is not null && second is not null && first.Equals(second);
        }

        public static bool operator !=(Entity? first, Entity? second)
        {
            return !(first == second);
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}
