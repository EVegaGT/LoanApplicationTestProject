using Domain.Interfaces;
using Domain.Models;
using Domain.Policies.Interfaces;

namespace Domain.Policies.Services
{
    public class LoanRequestDecisionService : ILoanRequestDecisionService
    {
        private readonly IEnumerable<ILoanRuleEvaluator> _rules;

        public LoanRequestDecisionService(IEnumerable<ILoanRuleEvaluator> rules)
        {
            _rules = rules;
        }

        public async Task<RuleDecisionResult> EvaluateLoanApplicationAsync(Customer customer)
        {
            foreach (var rule in _rules)
            {
                var result = await rule.EvaluateAsync(customer);
                if (!result.IsApproved) return result;
            }
            return new RuleDecisionResult(true);
        }
    }
}
