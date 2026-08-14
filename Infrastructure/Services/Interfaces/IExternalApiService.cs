namespace Infrastructure.Services.Interfaces
{
    public interface IExternalApiService
    {
        Task<bool> SyncLoanApplicationRequestAsync(string payload, bool isNewCustomer, CancellationToken cancellationToken);
    }
}