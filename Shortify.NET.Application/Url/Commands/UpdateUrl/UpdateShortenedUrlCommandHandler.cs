using Shortify.NET.Application.Abstractions;
using Shortify.NET.Application.Abstractions.Repositories;
using Shortify.NET.Application.Shared;
using Shortify.NET.Application.Shared.Models;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Core;
using Shortify.NET.Core.Errors;

namespace Shortify.NET.Application.Url.Commands.UpdateUrl
{
    internal sealed class UpdateShortenedUrlCommandHandler(
        IShortenedUrlRepository shortenedUrlRepository,
        ICachingServices cachingServices,
        IUnitOfWork unitOfWork)
        : ICommandHandler<UpdateShortenedUrlCommand, ShortenedUrlDto>
    {
        private readonly IShortenedUrlRepository _shortenedUrlRepository = shortenedUrlRepository;

        private readonly ICachingServices _cachingServices = cachingServices;
        
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        
        public async Task<Result<ShortenedUrlDto>> Handle(
            UpdateShortenedUrlCommand command, 
            CancellationToken cancellationToken = default)
        {
            var url = await _shortenedUrlRepository.GetByIdAsync(command.Id, cancellationToken);

            if (url is null) return Result.Failure<ShortenedUrlDto>(DomainErrors.ShortenedUrl.ShortenedUrlNotFound);
            
            url.Update(command.OriginalUrl, command.Title, command.Tags);
            _shortenedUrlRepository.Update(url);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new ShortenedUrlDto(
                Id: url.Id,
                UserId: url.UserId,
                OriginalUrl: url.OriginalUrl,
                ShortUrl: url.ShortUrl.Value,
                Code: url.Code,
                Title: url.Title,
                Tags: url.Tags,
                CreatedOnUtc: url.CreatedOnUtc,
                UpdatedOnUtc: url.UpdatedOnUtc,
                RowStatus: url.RowStatus
            );
            await SetCache(response, cancellationToken);
            
            return response; 
        }
        
        private async Task SetCache(
            ShortenedUrlDto cacheItem, 
            CancellationToken cancellationToken)
        {
            var cacheKey = $"{Constant.Cache.Prefixes.OriginalUrls}{cacheItem.Code}";

            await _cachingServices
                .SetAsync(
                    cacheKey,
                    cacheItem, 
                    cancellationToken: cancellationToken,
                    slidingExpiration: TimeSpan.FromDays(1));
        }
    }
}