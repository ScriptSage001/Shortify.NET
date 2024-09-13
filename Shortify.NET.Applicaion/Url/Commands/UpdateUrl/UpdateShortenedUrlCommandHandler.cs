using Shortify.NET.Applicaion.Abstractions.Repositories;
using Shortify.NET.Applicaion.Shared.Models;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Core;
using Shortify.NET.Core.Errors;

namespace Shortify.NET.Applicaion.Url.Commands.UpdateUrl
{
    internal sealed class UpdateShortenedUrlCommandHandler(
        IShortenedUrlRepository shortenedUrlRepository,
        IUnitOfWork unitOfWork)
        : ICommandHandler<UpdateShortenedUrlCommand, ShortenedUrlDto>
    {
        private readonly IShortenedUrlRepository _shortenedUrlRepository = shortenedUrlRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        
        public async Task<Result<ShortenedUrlDto>> Handle(
            UpdateShortenedUrlCommand command, 
            CancellationToken cancellationToken = default)
        {
            var url = await _shortenedUrlRepository.GetByIdAsync(command.Id, cancellationToken);

            if (url is null) return Result.Failure<ShortenedUrlDto>(DomainErrors.ShortenedUrl.ShortenedUrlNotFound);
            
            url.Update(command.Title, command.Tags);
            _shortenedUrlRepository.Update(url);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ShortenedUrlDto(
                Id: url.Id,
                UserId: url.UserId,
                OriginalUrl: url.OriginalUrl,
                ShortUrl: url.ShortUrl.Value,
                Code: url.Code,
                Title: url.Title,
                Tags: url.Tags,
                CreatedOnUtc: url.CreatedOnUtc,
                UpdatedOnUtc: url.UpdatedOnUtc
            );

        }
    }
}