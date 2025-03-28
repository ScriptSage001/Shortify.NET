﻿using FluentValidation;

namespace Shortify.NET.Application.Token.Commands.RefreshToken
{
    internal class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator() 
        {
            RuleFor(x => x)
                .NotEmpty();

            RuleFor(x => x.AccessToken)
                .NotEmpty();

            RuleFor(x => x.RefreshToken)
                .NotEmpty();
        }
    }
}
