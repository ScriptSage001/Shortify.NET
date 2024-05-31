using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using Shortify.NET.Applicaion.Helpers;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Applicaion
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

        private static IServiceCollection AddHelpers(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            return services;
        }

        private static IServiceCollection AddDomainEventHandlers(this IServiceCollection services)
        {
            services.Scan(
                selector => selector
                    .FromAssemblies(
                        AssemblyReference.Assembly
                    )
                    .AddClasses(classes =>
                        classes.Where(t =>
                            t.IsClass &&
                            !t.IsAbstract &&
                            t.GetInterfaces()
                                .Any(i =>
                                    i.IsGenericType &&
                                    i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>))))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsImplementedInterfaces()
                    .WithScopedLifetime());

            return services;
        }
    }
}
