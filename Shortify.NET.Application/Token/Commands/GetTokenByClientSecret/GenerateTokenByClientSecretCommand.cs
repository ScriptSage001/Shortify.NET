using Shortify.NET.Application.Shared.Models;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Application.Token.Commands.GetTokenByClientSecret
{
    public record GenerateTokenByClientSecretCommand(
        string UserName,
        string ClientSecret
        ) 
        : ICommand<AuthenticationResult>;
}
