using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Common.Messaging.Outbox;
using Shortify.NET.Core;
using Shortify.NET.Core.Primitives;

namespace Shortify.NET.Persistence
{
    public sealed class UnitOfWork(AppDbContext appDbContext) 
        : IUnitOfWork
    {
        private readonly AppDbContext _appDbContext = appDbContext;

        #region Public Methods

        /// <summary>
        /// Custom SaveChanges method on top of the SaveChanges of EFCore
        /// Handles Update of Auditable Entites before saving changes to DB
        /// </summary>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            InsertDomainEventsIntoOutboxMessages();
            UpdateAuditableEntities();

            return _appDbContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Asynchronously begins a new database transaction.
        /// </summary>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
        {
            return await _appDbContext
                            .Database
                            .BeginTransactionAsync(cancellationToken);
        }

        /// <summary>
        /// Disposes the context and releases all resources used by the UnitOfWork.
        /// </summary>
        public void Dispose()
        {
            _appDbContext.Dispose();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// To set Auditable Properties of the Auditable Enitites
        /// Using ChangeTracker of EF Core
        /// </summary>
        private void UpdateAuditableEntities()
        {
            var entities = _appDbContext.ChangeTracker.Entries<IAuditable>();

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    entity
                        .Property(x => x.CreatedOnUtc)
                        .CurrentValue = DateTime.UtcNow;

                    entity
                        .Property(x => x.RowStatus)
                        .CurrentValue = true;
                }

                if (entity.State == EntityState.Modified)
                {
                    entity
                        .Property(x => x.UpdatedOnUtc)
                        .CurrentValue = DateTime.UtcNow;
                }

                if (entity.State == EntityState.Deleted)
                {
                    entity.State = EntityState.Modified;

                    entity
                        .Property(x => x.RowStatus)
                        .CurrentValue = false;

                    entity
                        .Property(x => x.UpdatedOnUtc)
                        .CurrentValue = DateTime.UtcNow;
                }
            }
        }

        /// <summary>
        /// Converts DomainEvents into OutboxMessages
        /// Inserts OutboxMessages into the DB
        /// Using ChangeTracker of EF Core
        /// </summary>
        private void InsertDomainEventsIntoOutboxMessages()
        {
            var outboxMessages = _appDbContext
                                    .ChangeTracker
                                    .Entries<Entity>()
                                    .Select(entry => entry.Entity)
                                    .SelectMany(GetDomainEvents)
                                    .Select(CreateOutboxMessage)
                                    .ToList();

            _appDbContext.Set<OutboxMessage>().AddRange(outboxMessages);
        }

        private IReadOnlyCollection<IDomainEvent> GetDomainEvents(Entity entity)
        {
            var domainEvents = entity.GetDomainEvents();
            entity.ClearDomainEvents();
            return domainEvents;
        }

        private OutboxMessage CreateOutboxMessage(IDomainEvent domainEvent)
        {
            return new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = nameof(IDomainEvent),
                OccurredOnUtc = DateTime.UtcNow,
                Content = JsonConvert.SerializeObject(
                                            domainEvent,
                                            new JsonSerializerSettings
                                            {
                                                TypeNameHandling = TypeNameHandling.All
                                            })
            };
        }

        #endregion
    }
}
