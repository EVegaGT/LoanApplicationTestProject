using Domain.Models;

namespace Domain.Policies.Interfaces
{
    public interface ILoanRuleEvaluator
    {
        Task<RuleDecisionResult> EvaluateAsync(Customer customer);
    }
}
