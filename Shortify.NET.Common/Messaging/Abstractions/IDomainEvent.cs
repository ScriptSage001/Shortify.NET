namespace Shortify.NET.Common.Messaging.Abstractions
{
    /// <summary>
    /// Interface to Represent an Domain Event
    /// </summary>
    public interface IDomainEvent : IEvent
    {
        public Guid Id { get; init; }
    }
}
