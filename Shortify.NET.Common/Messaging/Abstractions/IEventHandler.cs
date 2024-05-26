using MediatR;

namespace Shortify.NET.Common.Messaging.Abstractions
{
    /// <summary>
    /// Marker Interface for an EventHandler
    /// Defines an EventHandler for a specific Event
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public interface IEventHandler<TEvent> : INotificationHandler<TEvent>
        where TEvent : IEvent
    {
    }
}
