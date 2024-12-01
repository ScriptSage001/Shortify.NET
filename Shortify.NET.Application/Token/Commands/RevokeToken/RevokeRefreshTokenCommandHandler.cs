using Shortify.NET.Application.Abstractions.Repositories;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Core;
using Shortify.NET.Core.Errors;

namespace Shortify.NET.Application.Token.Commands.RevokeToken
{
    internal sealed class RevokeRefreshTokenCommandHandler(
        IUserCredentialsRepository userCredentialsRepository,
        IUnitOfWork unitOfWork) 
        : ICommandHandler<RevokeRefreshTokenCommand>
    {
        private readonly IUserCredentialsRepository _userCredentialsRepository = userCredentialsRepository;

        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result> Handle(RevokeRefreshTokenCommand command, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(command.UserId);

            var userCreds = await _userCredentialsRepository
                                        .GetByUserIdAsync(
                                                userId,
                                                cancellationToken);

            if (userCreds is null)
            {
                return Result.Failure(DomainErrors.User.UserNotFound);
            }

            userCreds.AddOrUpdateRefreshToken(null, DateTime.UtcNow);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}