using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Application.Cache.Commands.ClearAll
{
    /// <summary>
    /// Represents a command to clear all cache.
    /// </summary>
    /// <remarks>
    /// This command is used to trigger the clearing of all cache entries. 
    /// It implements the <see cref="ICommand"/> interface, indicating it is part of the CQRS pattern.
    /// </remarks>
    public record ClearAllCacheCommand : ICommand;
}