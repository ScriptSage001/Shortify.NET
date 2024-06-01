namespace Shortify.NET.Core.Events
{
    public record PasswordChangedDomainEvent(
            Guid Id,
            Guid UserId
        ) 
        : DomainEvent(Id);
}
