using Shortify.NET.Application.Abstractions.Repositories;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Core.ValueObjects;

namespace Shortify.NET.Application.Users.Queries.IsEmailUnique;

public class IsEmailUniqueQueryHandler(
        IUserRepository userRepository
    ) : IQueryHandler<IsEmailUniqueQuery, bool>
{
    private readonly IUserRepository _userRepository = userRepository; 
    
    public async Task<Result<bool>> Handle(IsEmailUniqueQuery query, CancellationToken cancellationToken)
    {
        var email = Email.Create(query.Email);
        return await _userRepository.IsEmailUniqueAsync(email.Value, cancellationToken);
    }
}