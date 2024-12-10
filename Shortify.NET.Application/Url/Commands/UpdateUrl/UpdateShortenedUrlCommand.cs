using Shortify.NET.Application.Shared.Models;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Application.Url.Commands.UpdateUrl
{
    public record UpdateShortenedUrlCommand(
        Guid Id,
        string OriginalUrl,
        string? Title,
        List<string>? Tags)
        : ICommand<ShortenedUrlDto>;
}