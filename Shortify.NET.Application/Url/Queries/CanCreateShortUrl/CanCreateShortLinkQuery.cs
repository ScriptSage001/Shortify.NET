using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Application.Url.Queries.CanCreateShortUrl
{
    public record CanCreateShortLinkQuery(string UserId) : IQuery<bool>;
}
