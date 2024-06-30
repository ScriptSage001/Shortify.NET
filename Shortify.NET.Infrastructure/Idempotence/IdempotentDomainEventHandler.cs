using Microsoft.EntityFrameworkCore;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Common.Messaging.Outbox;
using Shortify.NET.Persistence;

namespace Shortify.NET.Infrastructure.Idempotence
{
    /// <summary>
    /// Decorator of DomainEventHandler
    /// To Implement Idempotence Consumption
    /// </summary>
    /// <typeparam name="TDomainEvent"></typeparam>
    public sealed class IdempotentDomainEventHandler<TDomainEvent>(
        IDomainEventHandler<TDomainEvent> decoratedHandler, 
        AppDbContext appDbContext) 
        : IDomainEventHandler<TDomainEvent>
        where TDomainEvent : IDomainEvent
    {
        private readonly IDomainEventHandler<TDomainEvent> _decoratedHandler = decoratedHandler;

        private readonly AppDbContext _appDbContext = appDbContext;

        public async Task Handle(TDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var consumer = _decoratedHandler.GetType().Name;

            var isEventConsumed = await _appDbContext.Set<OutboxMessageConsumer>()
                                            .AnyAsync(
                                                obmCon =>
                                                    obmCon.Id == domainEvent.Id &&
                                                    obmCon.Name == consumer,
                                                cancellationToken);
            if (isEventConsumed)
            {
                return;
            }

            await _decoratedHandler.Handle(domainEvent, cancellationToken);

            _appDbContext.Set<OutboxMessageConsumer>()
                    .Add(new OutboxMessageConsumer
                    {
                        Id = domainEvent.Id,
                        Name = consumer
                    });

            await _appDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
