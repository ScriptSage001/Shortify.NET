using FluentValidation;

namespace Shortify.NET.Application.Url.Queries.ShortenedUrl
{
    internal class GetShortenedUrlByIdQueryValidator : AbstractValidator<GetShortenedUrlByIdQuery>
    {
        public GetShortenedUrlByIdQueryValidator()
        {
            RuleFor(x => x).NotEmpty();

            RuleFor(x => x.Id)
                .NotNull();
        }
    }
}
