using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Application.Token.Commands.RevokeToken
{
    public record RevokeRefreshTokenCommand(string UserId) : ICommand;
}
