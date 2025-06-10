using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Tazkartk.Application.DTO.Payments;
using Tazkartk.Application.Interfaces;
using Tazkartk.Application.Services;

namespace Tazkartk.API.Controllers
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
        [HttpPost("/api/paymob/callback")]
        public async Task<IActionResult> CallBack()
        {
            var result = await _paymentService.handleCallback(Request);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("balance")]
        [SwaggerOperation(Summary="paymob balance")]
        public async Task<IActionResult> balance()
        {
            var res = await _paymentService.BalanceInquiry();
            return Ok(res);
        }
       
        [HttpGet]
        [SwaggerOperation(Summary = "List All Payments")]

        public async Task<IActionResult> GetPayments()
        {
            var payments = await _paymentService.GetAllPaymentsAsync();
            return Ok(payments);
        }

    }

}
