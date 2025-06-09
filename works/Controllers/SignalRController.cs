using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using works.Hubs;

namespace works.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SignalRController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public SignalRController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage",
                request.User, request.Message, DateTime.Now);

            return Ok(new { success = true, message = "Message sent successfully" });
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new
            {
                message = "ChatRoom API is working",
                timestamp = DateTime.Now,
                hubEndpoint = "/chathub"
            });
        }
    }

   
}