namespace Infrastructure.BackgroundJobs.Payloads
{
    public class RequestLoanExternalPayload
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Ssn { get; set; } = string.Empty;
    }
}
