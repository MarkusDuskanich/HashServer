using BL.Hash;
using BL.HashApi.Payloads;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Models;

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
            //fix message in db, in context and here

            if (hashRequest.Message == null)
                return BadRequest();
            var hashValue = _hash.ComputeHash(hashRequest.Message);
            using var hashServerDbContext = new HashServerDbContext();
            var existingHash = (from e in hashServerDbContext.Hashes where e.HashValue == hashValue select e).FirstOrDefault();
            if (existingHash != null)
            {
                if(existingHash.Messages.Any(message => message. == hashRequest.Message))
                hashServerDbContext.Messages.Add(new()
                {
                    Id = Guid.NewGuid(),
                    Hash = existingHash,
                    Hashid = existingHash.Id
                });
            }
            else
            {
                var hashId = Guid.NewGuid();
                var newHash = new Models.Hash()
                {
                    Id = hashId,
                    HashValue = hashValue,
                };
                var newMessage = new Models.Message()
                {
                    Hash = newHash,
                    Hashid = hashId,
                    Id = Guid.NewGuid(),
                };
                newHash.Messages = new List<Message>()
                { 
                    newMessage
                };
                hashServerDbContext.Hashes.Add(newHash);
                hashServerDbContext.Messages.Add(newMessage);
            }


            return Ok(hashValue);
        }
    }
}