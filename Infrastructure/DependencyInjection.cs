using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //SQLite configuration
            services.AddDbContext<DataBaseContext.AppDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

            // Register repositories
            services.AddScoped<Domain.Interfaces.ILoadRepository, Repositories.LoadRepository>();
            return services;
        }
    }
}
