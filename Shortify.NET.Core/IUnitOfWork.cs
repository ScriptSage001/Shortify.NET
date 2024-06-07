using Microsoft.EntityFrameworkCore.Storage;

namespace Shortify.NET.Core
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Asynchronously begins a new database transaction.
        /// </summary>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously begins a new database transaction.
        /// </summary>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the started transaction.</returns>
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
    }
}
