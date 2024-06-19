using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Shortify.NET.Applicaion.Abstractions;
using Shortify.NET.Infrastructure.Helpers;
using Shortify.NET.Infrastructure.Idempotence;

namespace Shortify.NET.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHelpers(configuration);
            services.AddServices();
            services.AddBackgroundJobs(configuration);
            services.AddMiddleware();
            services.AddCaching();

            return services;
        }

        private static void AddHelpers(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
            services.Configure<ShortLinkSettings>(configuration.GetSection("ShortLinkSettings"));
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        }

        private static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthServices, AuthServices>();
            services.AddScoped<IUrlShorteningService, UrlShorteningService>();
            services.AddSingleton<IEmailServices, EmailServices>();
        }

        private static void AddBackgroundJobs(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddQuartz(configure =>
            {
                List<BackgroundJobConfig>? backgroundJobs = configuration.GetSection("BackgroundJobs").Get<List<BackgroundJobConfig>>();

                if (backgroundJobs is not null && backgroundJobs.Count != 0)
                {
                    foreach (var backgroundJob in backgroundJobs)
                    {
                        if (backgroundJob.Enabled)
                        {
                            Type? jobType = AssemblyReference.Assembly.GetType($"Shortify.NET.Infrastructure.BackgroudJobs.{backgroundJob.Name}");

                            if (jobType is not null)
                            {
                                var jobKey = new JobKey(backgroundJob.Name);
                                var triggerKey = backgroundJob.Name + "Trigger";

                                configure.AddJob(jobType, jobKey);

                                configure.AddTrigger(trigger =>
                                                        trigger
                                                            .WithIdentity(triggerKey)
                                                            .ForJob(jobKey)
                                                            .WithCronSchedule(backgroundJob.Schedule));
                            }
                        }
                    }
                }
            });

            services.AddQuartzHostedService();
        }

        private static void AddMiddleware(this IServiceCollection services)
        {
            services.Decorate(typeof(INotificationHandler<>), typeof(IdempotentDomainEventHandler<>));
        }

        private static void AddCaching(this IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddSingleton<ICachingServices, CachingServices>();
        }
    }
}
