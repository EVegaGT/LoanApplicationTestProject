namespace ExternalServiceProject.Payloads
{
    public class LoanApplication
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public decimal Amount { get; set; }
        public string Ssn { get; set; }
    }
}
