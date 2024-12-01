using Shortify.NET.Application.Abstractions;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Application.Cache.Commands.ClearByPrefix
{
    /// <summary>
    /// Handles the <see cref="ClearCacheByPrefixCommand"/> to clear cache entries that match a specific prefix.
    /// </summary>
    /// <remarks>
    /// This command handler uses the <see cref="ICachingServices"/> to perform the actual cache clearing operation.
    /// It implements the <see cref="ICommandHandler{ClearCacheByPrefixCommand}"/> interface, which is part of the CQRS pattern.
    /// </remarks>
    internal sealed class ClearCacheByPrefixCommandHandler(ICachingServices cachingServices)
        : ICommandHandler<ClearCacheByPrefixCommand>
    {
        private readonly ICachingServices _cachingServices = cachingServices;

        /// <summary>
        /// Handles the <see cref="ClearCacheByPrefixCommand"/> to clear cache entries
        /// that match a specific prefix asynchronously.
        /// </summary>
        /// <param name="command">The clear cache by prefix command.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A <see cref="Result"/> indicating the success of the operation.</returns>
        public async Task<Result> Handle(
            ClearCacheByPrefixCommand command, 
            CancellationToken cancellationToken = default)
        {
            await _cachingServices.RemoveByPrefixAsync(command.Prefix, cancellationToken);
            
            return Result.Success();
        }
    }
}