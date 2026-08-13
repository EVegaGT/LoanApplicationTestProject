using LoanTestProject.Payload;
using Microsoft.AspNetCore.Mvc;

namespace LoanTestProject.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class LoanApplicationController : ControllerBase
    {
        // POST: api/LoanApplication
        [HttpPost]
        public IActionResult Post([FromBody] LoanApplication application)
        {
            //This just an Mock external service, We will return Ok always
            return Ok();
        }
    }
}
