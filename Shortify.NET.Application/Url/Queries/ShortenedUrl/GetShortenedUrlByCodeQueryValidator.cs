using FluentValidation;

namespace Shortify.NET.Application.Url.Queries.ShortenedUrl
{
    internal class GetShortenedUrlByCodeQueryValidator : AbstractValidator<GetShortenedUrlByCodeQuery>
    {
        public GetShortenedUrlByCodeQueryValidator()
        {
            RuleFor(x => x).NotEmpty();

            RuleFor(x => x.Code)
                .NotNull()
                .Length(7);
        }
    }
}
