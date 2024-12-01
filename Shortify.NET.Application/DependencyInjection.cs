using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using Shortify.NET.Application.Helpers;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(AssemblyReference.Assembly, includeInternalTypes: true);
            services.AddHelpers(configuration);
            services.AddDomainEventHandlers();

            return services;
        }

        private static void AddHelpers(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        }

        private static void AddDomainEventHandlers(this IServiceCollection services)
        {
            services.Scan(
                selector => selector
                    .FromAssemblies(
                        AssemblyReference.Assembly
                    )
                    .AddClasses(classes =>
                        classes.Where(t =>
                            t is { IsClass: true, IsAbstract: false } &&
                            t.GetInterfaces()
                                .Any(i =>
                                    i.IsGenericType &&
                                    i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>))))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsImplementedInterfaces()
                    .WithScopedLifetime());
        }
    }
}
