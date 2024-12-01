using Shortify.NET.Application.Shared.Models;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Application.Users.Commands.LoginUser
{
    public sealed record LoginUserCommand(
        string? UserName,
        string? Email,
        string Password
    ) : ICommand<AuthenticationResult>;
}
