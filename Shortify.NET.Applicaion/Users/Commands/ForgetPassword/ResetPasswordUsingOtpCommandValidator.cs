using FluentValidation;
using Shortify.NET.Core.ValueObjects;

namespace Shortify.NET.Applicaion.Users.Commands.ForgetPassword
{
    internal class ResetPasswordUsingOtpCommandValidator : AbstractValidator<ResetPasswordUsingOtpCommand>
    {
        public ResetPasswordUsingOtpCommandValidator()
        {
            RuleFor(x => x)
                .NotEmpty();

            RuleFor(x => x.Email)
                .NotEmpty()
                .MaximumLength(Email.MaxLength);               

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(30);

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(30);

            RuleFor(x => x)
                .Must(
                    command =>
                        command.NewPassword.Equals(command.ConfirmPassword))
                .WithMessage("NewPassword and ConfirmPassword must be same.");
        }
    }
}
