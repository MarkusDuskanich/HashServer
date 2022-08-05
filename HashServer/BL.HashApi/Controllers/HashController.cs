using BL.Hash.SHA256;
using BL.HashApi.Payloads;
using Microsoft.AspNetCore.Mvc;

namespace BL.HashApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HashController : ControllerBase
    {

        private readonly ILogger<HashController> _logger;

        public HashController(ILogger<HashController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult CreateHash([FromBody] HashRequestBody hashRequest)
        {
            if (hashRequest.Message == null)
                return BadRequest();
            return Ok(new SHA256(hashRequest.Message).Hash());
        }
    }
}