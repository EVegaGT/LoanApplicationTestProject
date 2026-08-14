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
            if (customer.Application == null)
            {
                throw new ArgumentNullException(nameof(Application), "Customer must have an associated Application.");
            }

            if (customer.Id == 0)
            {
                _context.Customers.Add(customer);
            }

            // Save changes to the database
            // for update is not necessary to call _context.Update(customer) because the customer is already being tracked by the context,
            // and any changes made to its properties will be automatically detected and persisted when SaveChangesAsync() is called.
            await _context.SaveChangesAsync();
        }
    }
}
