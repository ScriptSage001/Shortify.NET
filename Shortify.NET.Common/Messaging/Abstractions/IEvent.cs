using MediatR;

namespace Shortify.NET.Common.Messaging.Abstractions
{
    /// <summary>
    /// Marker Interface to Reperesent an Event
    /// </summary>
    public interface IEvent : INotification
    {
    }
}
