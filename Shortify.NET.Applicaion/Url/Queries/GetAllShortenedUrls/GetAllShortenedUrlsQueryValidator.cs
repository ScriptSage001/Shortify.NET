using FluentValidation;

namespace Shortify.NET.Applicaion.Url.Queries.GetAllShortenedUrls
{
    public class GetAllShortenedUrlsQueryValidator : AbstractValidator<GetAllShortenedUrlsQuery>
    {
        public GetAllShortenedUrlsQueryValidator()
        {
            RuleFor(x => x)
                .NotEmpty();

            RuleFor(x => x.UserId)
                .NotEmpty();
        }
    }
}
