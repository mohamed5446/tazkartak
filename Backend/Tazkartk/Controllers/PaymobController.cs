using Microsoft.AspNetCore.Mvc;
using Tazkartk.Application.Interfaces;
using Tazkartk.Application.DTO.Payments;

namespace Tazkartk.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymobController : ControllerBase
    {
        private readonly IPaymentService _PaymentService;
        public PaymobController(IPaymentService paymentService)
        {
            _PaymentService = paymentService;
        }
        //[HttpGet("balance")]
        //public async Task<IActionResult> balance()
        //{
        //    var res = await _PaymentService.BalanceInquiry();
        //    return Ok(res);
        //}
        //[HttpPost("dispurse")]
        //public async Task<IActionResult> diss(withdrawldto DTO)
        //{
        //    var res = await _PaymentService.DispurseAsync( DTO.Issuer,DTO.walletnumber,DTO.amount);
        //    return Ok(res);
        //}
        //[HttpPost("accesstoken")]
        //public async Task<IActionResult> GenerateAccessToken()
        //{
        //    var res = await _PaymentService.Genrateaccesstoken();
        //    return Ok(res);
        //}
        [HttpPost("callback")]
        public async Task<IActionResult> CallBack()
        {
            var result = await _PaymentService.handleCallback(Request);
            return StatusCode((int)result.StatusCode, result);
        }
    }

}

