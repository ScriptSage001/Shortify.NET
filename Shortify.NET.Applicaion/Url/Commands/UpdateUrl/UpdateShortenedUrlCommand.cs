using Shortify.NET.Applicaion.Shared.Models;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Applicaion.Url.Commands.UpdateUrl
{
    public record UpdateShortenedUrlCommand(
        Guid Id,
        string? Title,
        List<string>? Tags)
        : ICommand<ShortenedUrlDto>;
}