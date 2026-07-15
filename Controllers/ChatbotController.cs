using Microsoft.AspNetCore.Mvc;
using DAShopTech.Services;
using System.Threading.Tasks;

namespace DAShopTech.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly ChatGPTService _chatGPTService;

        public ChatbotController(ChatGPTService chatGPTService)
        {
            _chatGPTService = chatGPTService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ChatMessage message)
        {
            var reply = await _chatGPTService.GetResponseAsync(message.Message);
            return Ok(new { reply = reply });
        }
    }

    public class ChatMessage
    {
        public string Message { get; set; }
    }
}
