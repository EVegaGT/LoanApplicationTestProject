using Domain.EventsContracts;
using Domain.Interfaces;
using Domain.Models;
using Infrastructure.DataBaseContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class LoadRepository : ILoadRepository
    {
        private readonly AppDbContext _context;

        public LoadRepository(AppDbContext context)
        {
            _context = context;
        }

        // Return Customer by SSN, if not found return null
        public async Task<Customer?> GetCustomerBySsn(string ssn)
        {
            var customer = await _context.Customers
                .Include(x => x.Application)
                .FirstOrDefaultAsync(c => c.Ssn == ssn);
            return customer;
        }

        //Save Customer and Application in a single transaction
        //If the customer already exists, update the existing record, otherwise add a new record
        //If customer or application fails to save, the entire transaction will be rolled back, due to the use of SaveChangesAsync() which is atomic from Entity Framework Core
        public async Task SaveApplicationTransactionAsync(Customer customer)
        {
            var isNewCustomer = customer.Id == 0; // Check if the customer is new (Id is 0 for new entities)

            if (customer.Application == null)
            {
                throw new ArgumentNullException(nameof(Application), "Customer must have an associated Application.");
            }

            if (isNewCustomer)
            {
                _context.Customers.Add(customer);
            }

            // we save the event as JSON in the OutboxMessages table, so that it can be processed later by a background service or a message broker
            // The Outbox pattern is used to ensure that the event is only published if the transaction is successful, and to provide a reliable way to retry publishing the event if it fails
            #region Outbox Message Creation
            // Create the event payload for the outbox message
            var integrationEvent = new LoadRequestEventPayload
            {
                Ssn = customer.Ssn,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Address = customer.Address,
                State = customer.State,
                CompanyName = customer.CompanyName,
                RequestedAmount = customer.Application.RequestedAmount,
                IsNewCustomer = isNewCustomer
            };

            var outboxMessage = new OutboxMessage
            {
                EventType = "LoadRequestSaved",
                Payload = System.Text.Json.JsonSerializer.Serialize(integrationEvent)
            };

            _context.OutboxMessages.Add(outboxMessage);
            #endregion


            // Save changes to the database
            // SaveChangesAsync() will handle both the customer/application and the outbox message in a single transaction
            // for update is not necessary to call _context.Update(customer) because the customer is already being tracked by the context,
            // and any changes made to its properties will be automatically detected and persisted when SaveChangesAsync() is called.
            await _context.SaveChangesAsync();
        }
    }
}
