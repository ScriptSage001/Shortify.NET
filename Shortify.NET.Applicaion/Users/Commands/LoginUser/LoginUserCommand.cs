using Shortify.NET.Applicaion.Shared.Models;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Applicaion.Users.Commands.LoginUser
{
    public sealed record LoginUserCommand(
        string UserName,
        string Password
    ) : ICommand<AuthenticationResult>
    {
    }
}
