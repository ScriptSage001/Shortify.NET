using FluentValidation;
using Shortify.NET.Core.ValueObjects;

namespace Shortify.NET.Application.Users.Queries.IsEmailUnique;

internal class IsEmailUniqueQueryValidator : AbstractValidator<IsEmailUniqueQuery>
{
    public IsEmailUniqueQueryValidator()
    {
        RuleFor(q => q).NotEmpty();
        
        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(Email.MaxLength);
    }
}