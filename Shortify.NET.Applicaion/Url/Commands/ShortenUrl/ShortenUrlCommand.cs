using Microsoft.AspNetCore.Http;
using Shortify.NET.Applicaion.Shared.Models;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Core.ValueObjects;

namespace Shortify.NET.Applicaion.Url.Commands.ShortenUrl
{
    public record ShortenUrlCommand(
        string Url,
        HttpRequest HttpRequest ) : ICommand<ShortUrl>;
}
