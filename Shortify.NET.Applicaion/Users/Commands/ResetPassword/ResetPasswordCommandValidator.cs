using FluentValidation;

namespace Shortify.NET.Applicaion.Users.Commands.ResetPassword
{
    internal class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator() 
        {
            RuleFor(x => x)
                .NotEmpty();

            RuleFor(x => x)
               .Must(
                    command => 
                        !string.IsNullOrWhiteSpace(command.UserId));

            RuleFor(x => x.OldPassword)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(30);

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

            RuleFor(x => x)
                .Must(
                    command =>
                        !command.NewPassword.Equals(command.OldPassword))
                .WithMessage("NewPassword and OldPassword must not be same.");
        }
    }
}
