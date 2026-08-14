namespace Domain.Policies
{
    public record RuleDecisionResult(bool IsApproved, string DenialReason = "");
   
}