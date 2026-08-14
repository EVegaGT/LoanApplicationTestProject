using Domain.Models;
using Domain.Policies.Interfaces;

namespace Domain.Policies.Rules
{
    public class StateDenyRule : ILoanRuleEvaluator
    {
        // The requirement specifies that we should deny loans for customers in New York (NY).
        // For now, we will hardcode a list of denied states.
        private static readonly HashSet<string> DeniedStates = new (StringComparer.OrdinalIgnoreCase) { "NY" };

        public Task<RuleDecisionResult> EvaluateAsync(Customer customer)
        {
            if (DeniedStates.Contains(customer.State.ToLower()))
                return Task.FromResult(new RuleDecisionResult(false, "We do not operate in this state."));

            return Task.FromResult(new RuleDecisionResult(true));
        }
    }
}
