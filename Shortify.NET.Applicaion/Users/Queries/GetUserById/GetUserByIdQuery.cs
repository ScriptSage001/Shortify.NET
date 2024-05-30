using Shortify.NET.Applicaion.Shared.Models;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Applicaion.Users.Queries.GetUserById
{
    public record GetUserByIdQuery(Guid UserId) : IQuery<UserDto>;
}
