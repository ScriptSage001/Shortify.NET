using Shortify.NET.Application.Abstractions.Repositories;
using Shortify.NET.Application.Shared.Models;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Core.Entites;
using Shortify.NET.Core.Errors;

namespace Shortify.NET.Application.Users.Queries.GetUserById
{
    internal sealed class GetUserByIdQueryHandler(IUserRepository userRepository) 
        : IQueryHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<UserDto>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(query.UserId, cancellationToken);

            if (user is null)
            {
                return Result.Failure<UserDto>(DomainErrors.User.UserNotFound);
            }

            return new UserDto(
                            Id: user.Id,
                            UserName: user.UserName.Value,
                            Email: user.Email.Value,
                            RowStatus: user.RowStatus,
                            CreatedOnUtc: user.CreatedOnUtc,
                            UpdatedOnUtc: user.UpdatedOnUtc);
        }
    }
}
