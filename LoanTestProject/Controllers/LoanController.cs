using Application.DTOS;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LoanTestProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly ILoanService _loanService;

        public LoanController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        //GET: api/Loan/customer/{ssn}
        [HttpGet("customer/{ssn}")]
        public async Task<IActionResult> GetCustomerBySsn(string ssn)
        {
            var customer = await _loanService.GetCustomerBySSNAsync(ssn);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        //POST: api/Loan/request
        [HttpPost("request")]
        public async Task<IActionResult> RequestLoan (RequestLoanApplication requestLoanApplication)
        {
            await _loanService.ProcessApplicationAsync(requestLoanApplication);
            return Ok();
        }
    }
}
