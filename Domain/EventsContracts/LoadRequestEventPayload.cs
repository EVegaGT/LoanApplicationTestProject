namespace Domain.EventsContracts
{
    public class LoadRequestEventPayload
    {
        public string Ssn { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public decimal RequestedAmount { get; set; }
        public bool IsNewCustomer { get; set; }
    }
}