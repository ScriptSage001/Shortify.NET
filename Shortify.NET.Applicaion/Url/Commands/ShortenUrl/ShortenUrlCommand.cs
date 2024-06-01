using Microsoft.AspNetCore.Http;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Core.ValueObjects;

namespace Shortify.NET.Applicaion.Url.Commands.ShortenUrl
{
    public record ShortenUrlCommand(
        string Url,
        string UserId,
        string? Title,
        List<string>? Tags,
        HttpRequest HttpRequest ) : ICommand<ShortUrl>;
}
