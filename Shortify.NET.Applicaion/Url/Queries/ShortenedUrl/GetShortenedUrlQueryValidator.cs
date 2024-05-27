using FluentValidation;

namespace Shortify.NET.Applicaion.Url.Queries.ShortenedUrl
{
    internal class GetShortenedUrlQueryValidator : AbstractValidator<GetShortenedUrlQuery>
    {
        public GetShortenedUrlQueryValidator()
        {
            RuleFor(x => x).NotEmpty();

            RuleFor(x => x.Code)
                .NotNull()
                .Length(7);
        }
    }
}
