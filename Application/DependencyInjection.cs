using Application.Services;
using Application.Services.Interfaces;
using Domain.Interfaces;
using Domain.Policies.Interfaces;
using Domain.Policies.Rules;
using Domain.Policies.Services;
using Microsoft.Extensions.DependencyInjection;
namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInApplication(this IServiceCollection services)
        {
            //register services
            services.AddScoped<ILoanService, LoanService>();
            services.AddScoped<ILoanRequestDecisionService, LoanRequestDecisionService>();

            //register domain rules
            services.AddScoped<ILoanRuleEvaluator, StateDenyRule>();

            return services;
        }

    }
}