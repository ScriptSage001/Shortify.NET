using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shortify.NET.Common.Behaviour;
using Shortify.NET.Common.Messaging;
using Shortify.NET.Common.Messaging.Abstractions;
using System.Reflection;

namespace Shortify.NET.Common
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddShortifyNetCommon(this IServiceCollection services, Assembly handlerAssembly)
        {
            AddMessaging(services, handlerAssembly);
            AddBehaviours(services);
            AddExceptionHandler(services);

            return services;
        }

        private static void AddMessaging(this IServiceCollection services, Assembly handlerAssembly)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(handlerAssembly));
            services.AddScoped<IApiService, ApiService>(c =>
            {
                var mediator = c.GetRequiredService<IMediator>();
                return new ApiService(mediator);
            });
        }

        private static void AddBehaviours(this IServiceCollection services)
        {
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehaviour<,>));
        }

        private static void AddExceptionHandler(this IServiceCollection services)
        {
            services.AddExceptionHandler<GlobalExceptionHandler>();
        }
    }
}
