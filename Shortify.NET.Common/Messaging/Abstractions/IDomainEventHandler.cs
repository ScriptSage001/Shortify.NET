namespace Shortify.NET.Common.Messaging.Abstractions
{
    /// <summary>
    /// Marker Interface for DomainEventHandlers
    /// Defines an DomainEventHandler for a specific DomainEvent
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public interface IDomainEventHandler<TEvent> : IEventHandler<TEvent>
        where TEvent : IDomainEvent
    {
    }
}
