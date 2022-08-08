using BL.HashApi.Payloads;
using Microsoft.AspNetCore.Mvc;

namespace BL.HashApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class MessagesController : ControllerBase
    {
        private readonly ILogger<HashController> _logger;

        public MessagesController(ILogger<HashController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{hash}")]
        public IActionResult GetMessagesFromHash([FromRoute] string hash)
        {
            return Ok(hash);
        }
    }
}
