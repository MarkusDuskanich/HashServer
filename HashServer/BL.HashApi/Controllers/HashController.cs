using BL.Hash;
using BL.HashApi.Payloads;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            if (hashRequest.Message == null)
                return BadRequest();
            var hashValue = _hash.ComputeHash(hashRequest.Message);
            using var hashServerDbContext = new HashServerDbContext();

            var existingHash = (from e in hashServerDbContext.Hashes where e.Value == hashValue select e).FirstOrDefault();
            if (existingHash != null)
            {
                foreach (var item in existingHash.Messages)
                {
                    _logger.Log(LogLevel.Information, $"{item}");
                }
                if (existingHash.Messages.Any(message => message.Value == hashRequest.Message))
                {
                    return Ok(hashValue);
                }

                hashServerDbContext.Messages.Add(new()
                {
                    Id = Guid.NewGuid(),
                    Hash = existingHash,
                    Value = hashRequest.Message,
                    Hashid = existingHash.Id
                });
            }
            else
            {
                var hashId = Guid.NewGuid();
                var newHash = new Models.Hash()
                {
                    Id = hashId,
                    Value = hashValue,
                };
                var newMessage = new Message()
                {
                    Hashid = hashId,
                    Value = hashRequest.Message,
                    Id = Guid.NewGuid(),
                };

                hashServerDbContext.Hashes.Add(newHash);
                hashServerDbContext.Messages.Add(newMessage);
            }

            hashServerDbContext.SaveChanges();
            return Ok(hashValue);
        }
    }
}