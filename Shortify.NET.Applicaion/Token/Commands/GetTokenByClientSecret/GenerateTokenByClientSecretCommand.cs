using Shortify.NET.Applicaion.Shared.Models;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Applicaion.Token.Commands.GetTokenByClientSecret
{
    public record GenerateTokenByClientSecretCommand(
        string UserName,
        string ClientSecret
        ) 
        : ICommand<AuthenticationResult>;
}
