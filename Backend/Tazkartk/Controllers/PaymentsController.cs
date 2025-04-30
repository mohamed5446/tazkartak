using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Tazkartk.Interfaces;

namespace Tazkartk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "List All Payments")]

        public async Task<IActionResult>GetPayments()
        {
            var payments=await _paymentService.GetAllPaymentsAsync();
            return Ok(payments);
        }
    }

}
