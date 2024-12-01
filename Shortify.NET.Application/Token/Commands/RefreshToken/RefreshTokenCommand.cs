using Shortify.NET.Application.Shared.Models;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Application.Token.Commands.RefreshToken
{
    public record RefreshTokenCommand(
        string AccessToken,
        string RefreshToken
        ) 
        : ICommand<AuthenticationResult>;
}
