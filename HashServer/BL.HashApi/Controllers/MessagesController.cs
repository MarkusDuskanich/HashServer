using BL.HashApi.Payloads;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;

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
            using var hashServerDbContext = new HashServerDbContext();
            var messages = (from h in hashServerDbContext.Hashes where h.Value == hash select h.Messages).FirstOrDefault();
           
            if (messages == null)
                return NotFound();

            var result = from m in messages select new MessageDTO(m);
            return Ok(result);
        }
    }
}
