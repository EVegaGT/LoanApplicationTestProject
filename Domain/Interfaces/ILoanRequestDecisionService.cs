using Domain.Models;
using Domain.Policies;

namespace Domain.Interfaces
{
    public interface ILoanRequestDecisionService
    {
        public Task<RuleDecisionResult> EvaluateLoanApplicationAsync(Customer customer);
    }
}
