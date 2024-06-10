using Shortify.NET.Applicaion.Shared.Models;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Applicaion.Users.Commands.RegisterUser
{
    public sealed record RegisterUserCommand(
        string UserName,
        string Email,
        string Password
        ) : ICommand<AuthenticationResult>;
}
