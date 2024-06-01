using FluentValidation;
using Shortify.NET.Core.ValueObjects;

namespace Shortify.NET.Applicaion.Token.Commands.GetTokenByClientSecret
{
    internal class GenerateTokenByClientSecretCommandValidator : AbstractValidator<GenerateTokenByClientSecretCommand>
    {
        public GenerateTokenByClientSecretCommandValidator()
        {
            RuleFor(x => x)
                .NotEmpty();

            RuleFor(x => x.UserName)
                .NotEmpty()
                .MaximumLength(UserName.MaxLength)
                .MinimumLength(UserName.MinLength);

            RuleFor(x => x.ClientSecret)
                .NotEmpty();
        }
    }
}