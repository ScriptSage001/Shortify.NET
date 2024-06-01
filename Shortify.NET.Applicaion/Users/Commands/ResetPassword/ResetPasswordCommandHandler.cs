﻿using Shortify.NET.Applicaion.Abstractions;
using Shortify.NET.Applicaion.Abstractions.Repositories;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Core;
using Shortify.NET.Core.Errors;

namespace Shortify.NET.Applicaion.Users.Commands.ResetPassword
{
    internal sealed class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand>
    {
        private readonly IUserCredentialsRepository _userCredentialsRepository;

        private readonly IAuthServices _authServices;

        private readonly IUnitOfWork _unitOfWork;

        public ResetPasswordCommandHandler(
            IUserCredentialsRepository userCredentialsRepository,
            IAuthServices authServices,
            IUnitOfWork unitOfWork)
        {
            _userCredentialsRepository = userCredentialsRepository;
            _authServices = authServices;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(command.UserId);

            var userCreds = await _userCredentialsRepository
                                        .GetByUserIdAsync(
                                                userId, 
                                                cancellationToken);

            if (userCreds is null)
            {
                return Result.Failure(DomainErrors.User.UserNotFound);
            }

            bool isVerified = _authServices
                                    .VerifyPasswordHash(
                                            command.OldPassword, 
                                            userCreds.PasswordHash, 
                                            userCreds.PasswordSalt);

            if (isVerified)
            {
                (byte[] passwordHash, byte[] passwordSalt) = _authServices.CreatePasswordHashAndSalt(command.NewPassword);

                userCreds.UpdatePasswordHashAndSalt(passwordHash, passwordSalt);

                _userCredentialsRepository.Update(userCreds);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            else
            {
                return Result.Failure(DomainErrors.UserCredentials.WrongCredentials);
            }
        }
    }
}