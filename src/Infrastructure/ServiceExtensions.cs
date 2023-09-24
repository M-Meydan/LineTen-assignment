using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Infrastructure.DbContexts;
using Microsoft.Extensions.Options;
using System.Runtime;
using Microsoft.IdentityModel.Tokens;

namespace Application
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddPersistence(configuration);
            // .AddLazyCache(); //Injects IAppCache 

        }

        /// <summary>
        /// Register all db persistence contexts here 
        /// </summary>
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("TestAppDb"), ServiceLifetime.Scoped);
            }
            else
            {
                var connectionString = configuration.GetConnectionString("Default");
                if (string.IsNullOrEmpty(connectionString))
                    throw new ArgumentNullException(nameof(connectionString));

                services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
                   connectionString, b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)),
                   ServiceLifetime.Scoped);
            }

            return services;
        }
    }
}
