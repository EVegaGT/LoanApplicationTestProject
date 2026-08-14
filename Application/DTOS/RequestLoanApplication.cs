namespace Application.DTOS
{
    public record RequestLoanApplication(
        string FirstName,
        string LastName,
        string Ssn,
        string Address,
        string State,
        string CompanyName,
        decimal RequestedAmount
    );
}