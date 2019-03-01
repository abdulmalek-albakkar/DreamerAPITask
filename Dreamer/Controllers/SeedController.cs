using Dreamer.IDomain.Repositories;
using Dreamer.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dreamer.API.Controllers
{
    [AllowAnonymous]
    [Produces("application/json")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class SeedController : DreamerBaseController
    {
        private ISeedingRepository SeedingRepository { get; set; }
        public SeedController(ISeedingRepository seedingRepository)
        {
            SeedingRepository = seedingRepository;
        }
        /// <summary>
        /// Seed some basic data, beside that it will remove the old data.
        /// </summary>
        [HttpGet]
        public IActionResult SeedDatabase()
        {
            if (SeedingRepository.Seed())
                return Ok();
            else
                return BadRequest();
        }
        /// <summary>
        /// Remove data from basic tabels.
        /// </summary>
        [HttpGet]
        public IActionResult ClearDatabase()
        {
            if (SeedingRepository.Clear())
                return Ok();
            else
                return BadRequest();
        }
    }
}