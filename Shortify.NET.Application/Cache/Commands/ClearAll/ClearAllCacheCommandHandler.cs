using Shortify.NET.Application.Abstractions;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Application.Cache.Commands.ClearAll
{
    /// <summary>
    /// Handles the <see cref="ClearAllCacheCommand"/> to clear all cache entries.
    /// </summary>
    /// <remarks>
    /// This command handler uses the <see cref="ICachingServices"/> to perform the actual cache clearing operation.
    /// It implements the <see cref="ICommandHandler{TCommand}"/> interface, which is part of the CQRS pattern.
    /// </remarks>
    internal sealed class ClearAllCacheCommandHandler(ICachingServices cachingServices)
        : ICommandHandler<ClearAllCacheCommand>
    {
        private readonly ICachingServices _cachingServices = cachingServices;

        /// <summary>
        /// Handles the <see cref="ClearAllCacheCommand"/> to clear all cache entries asynchronously.
        /// </summary>
        /// <param name="command">The clear all cache command request.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A <see cref="Result"/> indicating the success of the operation.</returns>
        public async Task<Result> Handle(
            ClearAllCacheCommand command, 
            CancellationToken cancellationToken = default)
        {
            await _cachingServices.ClearAllAsync(cancellationToken);

            return Result.Success();
        }
    }
}