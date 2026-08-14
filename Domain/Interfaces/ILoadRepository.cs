using Domain.Models;

namespace Domain.Interfaces
{
    public interface ILoadRepository
    {
        Task<Customer?> GetCustomerBySsn(string ssn);
        Task SaveApplicationTransactionAsync(Customer customer);
    }
}
