using Domain.Interfaces;
using Domain.Models;
using Infrastructure.DataBaseContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class OutboxMessageRepository : IOutboxMessageRepository
    {
        private readonly AppDbContext _context;

        public OutboxMessageRepository(AppDbContext context)
        {
            _context = context;
        }

        //We use batch pattern to retrieve a limited number of pending outbox messages,
        //which helps to avoid loading too many messages into memory at once and allows for better control over the processing of messages.
        public async Task<List<OutboxMessage>> GetPendingLoanOutboxMessagesAsync(int batchSize)
        {
            return await _context.OutboxMessages
                .Where(m => !m.Processed && m.EventType == "LoadRequestSaved" && !m.IsDeadLetter)
                .OrderBy(m => m.CreatedAt)
                .Take(batchSize)
                .ToListAsync();
        }

        public async Task UpdateOutboxMessageAsync(OutboxMessage message)
        {
            _context.OutboxMessages.Update(message);
            await _context.SaveChangesAsync();
        }
    }
}
