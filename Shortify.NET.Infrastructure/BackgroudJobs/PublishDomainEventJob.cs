using Polly.Retry;
using Polly;
using Quartz;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Common.Messaging.Outbox;
using Shortify.NET.Persistence;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Shortify.NET.Infrastructure.BackgroudJobs
{
    [DisallowConcurrentExecution]
    public class PublishDomainEventJob : IJob
    {
        private readonly AppDbContext _appDbContext;

        private readonly IApiService _apiService;

        public PublishDomainEventJob(AppDbContext appDbContext, IApiService apiService)
        {
            _appDbContext = appDbContext;
            _apiService = apiService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            List<OutboxMessage> outboxMessages = await _appDbContext
                                                            .Set<OutboxMessage>()
                                                            .Where(obm => obm.ProcessedOnUtc == null)
                                                            .Take(10)
                                                            .ToListAsync(context.CancellationToken);

            foreach (OutboxMessage outboxMessage in outboxMessages)
            {
                IDomainEvent? domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
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

                AsyncRetryPolicy retryPolicy = Policy
                                            .Handle<Exception>()
                                            .WaitAndRetryAsync(
                                                3,
                                                attempt => TimeSpan
                                                            .FromMilliseconds(100 * attempt));

                PolicyResult policyResult = await retryPolicy
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
