namespace Shortify.NET.Core.Events
{
    public record UserRegisteredDomainEvent(
                        Guid Id, 
                        Guid UserId, 
                        string Email) 
                  : DomainEvent(Id);
}
