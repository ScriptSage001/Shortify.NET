using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Applicaion.Url.Queries.GetOriginalUrl
{
    public record GetOriginalUrlQuery(string Code) : IQuery<string>;
}
