using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Core.Events
{
    public abstract record DomainEvent(Guid Id) : IDomainEvent;
}
