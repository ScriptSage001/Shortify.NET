using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shortify.NET.Application.Abstractions.Repositories;
using Shortify.NET.Core;
using Shortify.NET.Persistence.Repository;

namespace Shortify.NET.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAppDbContext(configuration);
            services.AddRepositories();
            services.AddServices();

            return services;
        }

        private static void AddAppDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Shortify.NETDB");
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        }

        private static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserCredentialsRepository, UserCredentialsRepository>();
            services.AddScoped<IShortenedUrlRepository, ShortenedUrlRepository>();
            services.AddScoped<IOtpRepository, OtpRepository>();
            services.AddScoped<RoleRepository>();
            services.AddScoped<IRoleRepository, CachedRoleRepository>();
        }

        private static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
