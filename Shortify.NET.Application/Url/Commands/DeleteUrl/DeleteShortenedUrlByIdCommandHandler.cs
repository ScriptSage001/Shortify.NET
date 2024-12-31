using Shortify.NET.Application.Abstractions;
using Shortify.NET.Application.Abstractions.Repositories;
using Shortify.NET.Application.Shared;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Core;
using Shortify.NET.Core.Entites;
using Shortify.NET.Core.Errors;

namespace Shortify.NET.Application.Url.Commands.DeleteUrl;

/// <summary>
/// Handler class for the <see cref="DeleteShortenedUrlByIdCommand"/> to delete a shortened URL by its identifier.
/// </summary>
internal sealed class DeleteShortenedUrlByIdCommandHandler(
    IShortenedUrlRepository shortenedUrlRepository,
    ICachingServices cachingServices,
    IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteShortenedUrlByIdCommand>
{
    private readonly IShortenedUrlRepository _shortenedUrlRepository = shortenedUrlRepository;
    private readonly ICachingServices _cachingServices = cachingServices;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <summary>
    /// Handles the command to delete a shortened URL by its identifier.
    /// </summary>
    /// <param name="command">The command containing the ID of the shortened URL to delete.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="Result"/> indicating the success or failure of the operation.</returns>
    public async Task<Result> Handle(DeleteShortenedUrlByIdCommand command, CancellationToken cancellationToken)
    {
        var url = await _shortenedUrlRepository.GetByIdAsync(command.Id, cancellationToken);
        if (url is null) return Result.Failure(DomainErrors.ShortenedUrl.ShortenedUrlNotFound);
        
        _shortenedUrlRepository.Delete(url);
        await RemoveFromCacheAsync(url, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    private async Task RemoveFromCacheAsync(ShortenedUrl shortenedUrl, CancellationToken cancellationToken)
    {
        var cacheKey = $"{Constant.Cache.Prefixes.OriginalUrls}{shortenedUrl.Code}";
        await _cachingServices.RemoveAsync(cacheKey, cancellationToken);
    }
}