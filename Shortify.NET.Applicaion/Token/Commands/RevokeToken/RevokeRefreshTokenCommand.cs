using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Applicaion.Token.Commands.RevokeToken
{
    public record RevokeRefreshTokenCommand(string UserId) : ICommand;
}
