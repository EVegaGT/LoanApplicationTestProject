using Domain.Policies.Interfaces;
using Domain.Policies.Rules;
using Infrastructure.BackgroundJobs;
using Infrastructure.Services;
using Infrastructure.Services.Interfaces;
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
            services.AddScoped<Domain.Interfaces.IOutboxMessageRepository, Repositories.OutboxMessageRepository>();

            // Register background service for processing outbox messages
            services.AddHostedService<OutboxLoanBackgroundService>();

            // Configure httpclient for external api
            var testApiUrl = configuration["ExternalServices:TestApiUrl"]
                    ?? throw new InvalidOperationException("Missing configuration: 'ExternalServices:TestApiUrl'");
            services.AddHttpClient<IExternalApiService, ExternalApiService>(client =>
            {
                client.BaseAddress = new Uri(testApiUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                // ignore SSL certificate validation for local development (not recommended for production)
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            });

            //register infrastructure rules
            services.AddScoped<ILoanRuleEvaluator, SSNBlacklistRule>();

            return services;
        }
    }
}
