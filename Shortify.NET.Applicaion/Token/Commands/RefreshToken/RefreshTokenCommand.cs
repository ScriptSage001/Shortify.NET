using Shortify.NET.Applicaion.Shared.Models;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Applicaion.Token.Commands.RefreshToken
{
    public record RefreshTokenCommand(
        string AccessToken,
        string RefreshToken
        ) 
        : ICommand<AuthenticationResult>;
}
