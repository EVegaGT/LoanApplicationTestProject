using Domain.Models;

namespace Domain.Interfaces
{
    public interface IOutboxMessageRepository
    {
        Task<List<OutboxMessage>> GetPendingLoanOutboxMessagesAsync(int batchSize);
        Task UpdateOutboxMessageAsync(OutboxMessage message);
    }
}
