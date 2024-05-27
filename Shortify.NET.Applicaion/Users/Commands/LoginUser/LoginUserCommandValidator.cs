using FluentValidation;
using Shortify.NET.Core.ValueObjects;

namespace Shortify.NET.Applicaion.Users.Commands.LoginUser
{
    internal class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator() 
        {
            RuleFor(x => x).NotEmpty();

            RuleFor(x => x.UserName)
                .NotEmpty()
                .MinimumLength(UserName.MinLength)
                .MaximumLength(UserName.MaxLength);

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(30);
        }
    }
}
