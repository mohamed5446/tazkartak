using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tazkartk.Application.DTO;
using Tazkartk.Application.Interfaces;

namespace Tazkartk.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessagesService _messagesService;

        public MessagesController(IMessagesService messagesService)
        {
            _messagesService = messagesService;
        }
        [HttpGet]
        public async Task<IActionResult>GetAllMessages()
        {
            var res= await _messagesService.GetAllMessages();
            return Ok(res);
        }
        [HttpPost]
        public async Task<IActionResult>SendMessage(MessageDTO DTO)
        {
            var res=await _messagesService.SendMessage(DTO);
            return StatusCode((int)res.StatusCode, res);
        }
    }
}
