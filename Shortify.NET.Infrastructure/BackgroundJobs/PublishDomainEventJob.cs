using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Polly;
using Quartz;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Common.Messaging.Outbox;
using Shortify.NET.Persistence;

namespace Shortify.NET.Infrastructure.BackgroundJobs
{
    [DisallowConcurrentExecution]
    public class PublishDomainEventJob(
        AppDbContext appDbContext, 
        IApiService apiService) 
        : IJob
    {
        private readonly AppDbContext _appDbContext = appDbContext;

        private readonly IApiService _apiService = apiService;

        public async Task Execute(IJobExecutionContext context)
        {
            var outboxMessages = await _appDbContext
                                                            .Set<OutboxMessage>()
                                                            .Where(obm => obm.ProcessedOnUtc == null)
                                                            .Take(10)
                                                            .ToListAsync(context.CancellationToken);

            foreach (var outboxMessage in outboxMessages)
            {
                var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                                                                    outboxMessage.Content,
                                                                    new JsonSerializerSettings
                                                                    {
                                                                        TypeNameHandling = TypeNameHandling.All
                                                                    });

                if (domainEvent is null)
                {
                    //log
                    continue;
                }

                var retryPolicy = Policy
                                            .Handle<Exception>()
                                            .WaitAndRetryAsync(
                                                3,
                                                attempt => TimeSpan
                                                            .FromMilliseconds(100 * attempt));

                var policyResult = await retryPolicy
                                                    .ExecuteAndCaptureAsync(() =>
                                                        _apiService
                                                            .EventPublisher(domainEvent, context.CancellationToken));

                if (policyResult.FinalException is not null)
                {
                    // log
                    outboxMessage.Error = policyResult.FinalException.ToString();
                }

                outboxMessage.ProcessedOnUtc = DateTime.UtcNow;
            }

            await _appDbContext.SaveChangesAsync();
        }
    }
}
