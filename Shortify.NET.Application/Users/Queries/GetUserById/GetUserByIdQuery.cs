using Shortify.NET.Application.Shared.Models;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Application.Users.Queries.GetUserById
{
    public record GetUserByIdQuery(Guid UserId) : IQuery<UserDto>;
}
