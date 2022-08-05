using BL.Hash;
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
        private readonly IHash _hash;
        public HashController(ILogger<HashController> logger, IHash hash)
        {
            _logger = logger;
            _hash = hash;
        }

        [HttpPost]
        public IActionResult CreateHash([FromBody] HashRequestBody hashRequest)
        {
            if (hashRequest.Message == null)
                return BadRequest();
            return Ok(_hash.ComputeHash(hashRequest.Message));
        }
    }
}