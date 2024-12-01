using FluentValidation;

namespace Shortify.NET.Application.Token.Commands.RevokeToken
{
    internal class RevokeRefreshTokenCommandValidator : AbstractValidator<RevokeRefreshTokenCommand>
    {
        public RevokeRefreshTokenCommandValidator() 
        {
            RuleFor(x => x)
                .NotEmpty();

            RuleFor(x => x.UserId)
                .NotEmpty();
        }
    }
}