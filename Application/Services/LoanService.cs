using Application.DTOS;
using Application.Services.Interfaces;
using Domain.Interfaces;
using Domain.Models;

namespace Application.Services
{
    public class LoanService : ILoanService
    {
        private readonly ILoadRepository _loadRepository;

        public LoanService(ILoadRepository loadRepository)
        {
            _loadRepository = loadRepository;
        }

        public async Task<RequestLoanApplication?> GetCustomerBySSNAsync(string ssn)
        {
           var customer = await _loadRepository.GetCustomerBySsn(ssn);
           if (customer == null) return null;

           return new RequestLoanApplication(
                customer.FirstName,
                customer.LastName,
                customer.Ssn,
                customer.Address,
                customer.State,
                customer.CompanyName,
                customer.Application?.RequestedAmount ?? 0m
           );
        }

        public async Task ProcessApplicationAsync(RequestLoanApplication request)
        {
            // check if customer exists
            var customer = await _loadRepository.GetCustomerBySsn(request.Ssn);

            // if customer does not exist, create a new customer and application
            if (customer == null)
            {

                customer = new Customer
                {
                    Ssn = request.Ssn,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Address = request.Address,
                    State = request.State,
                    CompanyName = request.CompanyName,
                    Application = new Domain.Models.Application
                    {
                        RequestedAmount = request.RequestedAmount
                    }
                };

            }
            // if customer exists, update the existing customer and application information
            else
            {
                customer.FirstName = request.FirstName;
                customer.LastName = request.LastName;
                customer.Address = request.Address;
                customer.State = request.State;
                customer.CompanyName = request.CompanyName;
                if (customer.Application == null)
                {
                    // we need to verift if the customer already has an application, if not we create a new one
                    customer.Application = new Domain.Models.Application
                    {
                        RequestedAmount = request.RequestedAmount
                    };
                }
                else
                {
                    // if the customer already has an application, we update the requested amount
                    customer.Application.RequestedAmount = request.RequestedAmount;
                }
            }

            await _loadRepository.SaveApplicationTransactionAsync(customer);
        }
    }
}
