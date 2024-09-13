using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Applicaion.Cache.Commands.ClearByPrefix
{
    /// <summary>
    /// Represents a command to clear cache entries that match a specific prefix.
    /// </summary>
    /// <param name="Prefix">The prefix of the cache keys to clear.</param>
    /// <remarks>
    /// This command is used to trigger the clearing of cache entries that match the specified key prefix.
    /// It implements the <see cref="ICommand"/> interface, indicating it is part of the CQRS pattern.
    /// </remarks>
    public record ClearCacheByPrefixCommand(string Prefix) : ICommand;
}