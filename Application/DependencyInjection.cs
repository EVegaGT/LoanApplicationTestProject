using Application.Services;
using Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInApplication(this IServiceCollection services)
        {
            //register services
            services.AddScoped<ILoanService, LoanService>();

            return services;
        }

    }
}
