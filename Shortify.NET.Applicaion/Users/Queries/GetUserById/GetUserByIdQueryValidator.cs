using FluentValidation;

namespace Shortify.NET.Applicaion.Users.Queries.GetUserById
{
    public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
    {
        public GetUserByIdQueryValidator()
        {
            RuleFor(x => x)
                .NotEmpty();

            RuleFor(x => x.UserId)
                .NotEmpty();
        }
    }
}
