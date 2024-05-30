using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Common.Messaging.Outbox;
using Shortify.NET.Core;
using Shortify.NET.Core.Primitives;

namespace Shortify.NET.Persistence
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _appDbContext;

        public UnitOfWork(AppDbContext appDbContext) => _appDbContext = appDbContext;

        /// <summary>
        /// Custom SaveChanges method on top of the SaveChanges of EFCore
        /// Handles Update of Auditable Entites before saving changes to DB
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            InsertDomainEventsIntoOutboxMessages();
            UpdateAuditableEntities();

            return _appDbContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// To set Auditable Properties of the Auditable Enitites
        /// Using ChangeTracker of EF Core
        /// </summary>
        private void UpdateAuditableEntities()
        {
            IEnumerable<EntityEntry<IAuditable>> entities = _appDbContext.ChangeTracker.Entries<IAuditable>();

            foreach (EntityEntry<IAuditable> entity in entities)
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
    }
}
