using FluentValidation;

namespace Shortify.NET.Applicaion.Url.Queries.ShortenedUrl
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
