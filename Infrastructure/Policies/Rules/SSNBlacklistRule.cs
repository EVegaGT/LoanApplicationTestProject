using Domain.Models;
using Domain.Policies.Interfaces;


namespace Domain.Policies.Rules
{
    public class SSNBlacklistRule : ILoanRuleEvaluator
    {
        //We mocked a list of blacklisted SSNs for demonstration purposes.
        //In a real-world scenario, this list would likely come from a database or external service.
        private static readonly HashSet<string> ssnBlackList = new(StringComparer.OrdinalIgnoreCase) { "784096895", "987654321" };

        public Task<RuleDecisionResult> EvaluateAsync(Customer customer)
        {
            if (ssnBlackList.Contains(customer.Ssn))
                return Task.FromResult(new RuleDecisionResult(false, "This SSN is blacklisted."));

            return Task.FromResult(new RuleDecisionResult(true));
        }
    }
}
