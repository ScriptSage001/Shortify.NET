using FluentValidation;

namespace Shortify.NET.Application.Otp.Commands.LoginUsingOtp
{
    public class LoginUsingOtpCommandValidator : AbstractValidator<LoginUsingOtpCommand>
    {
        public LoginUsingOtpCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                    .WithMessage("Email is required.")
                .EmailAddress()
                    .WithMessage("Email is not a valid email address.");

            RuleFor(x => x.Otp)
                .NotEmpty()
                    .WithMessage("OTP is required.")
                .Length(6)
                    .WithMessage("Provided OTP is invalid.");
        }
    }
}
