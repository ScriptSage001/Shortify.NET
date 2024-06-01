using FluentValidation;
using Shortify.NET.Core.ValueObjects;

namespace Shortify.NET.Applicaion.Users.Commands.LoginUser
{
    internal class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator()
        {
            RuleFor(x => x).NotEmpty();

            RuleFor(x => x)
                .Must(command => 
                       !string.IsNullOrWhiteSpace(command.UserName) 
                    || !string.IsNullOrWhiteSpace(command.Email))
                .WithMessage("Either UserName or Email must be provided.");

            RuleFor(x => x.UserName)
                .NotEmpty()
                .MinimumLength(UserName.MinLength)
                .MaximumLength(UserName.MaxLength)
                .When(x => !string.IsNullOrWhiteSpace(x.UserName));

            RuleFor(x => x.Email)
                 .NotEmpty()
                 .MaximumLength(Email.MaxLength)
                 .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(30);
        }
    }
}
