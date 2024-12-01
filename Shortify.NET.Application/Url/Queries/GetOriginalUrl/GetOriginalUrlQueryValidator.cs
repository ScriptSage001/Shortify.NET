using FluentValidation;

namespace Shortify.NET.Application.Url.Queries.GetOriginalUrl
{
    internal class GetOriginalUrlQueryValidator : AbstractValidator<GetOriginalUrlQuery>
    {
        public GetOriginalUrlQueryValidator()
        {
            RuleFor(x => x).NotEmpty();

            RuleFor(x => x.Code)
                .NotNull()
                .Length(7);
        }
    }
}
