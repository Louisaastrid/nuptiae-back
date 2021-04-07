using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Identity.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly ILogger<IdentityController> _logger;
        private readonly IIdentityRepo _repo;

        public IdentityController(IIdentityRepo repo, ILogger<IdentityController> logger)
        {
            _logger = logger;
            _repo = repo;
        }
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        /// <summary>
        /// Get a paginated list oof the catalog destinations
        /// </summary>
        /// <param name="pageNum"> Page Num</param>
        /// <param name="pageSize">Page Size </param>
        /// <returns>Paginated list of Catalog Destinations </returns>
        /// <response code="200">Request successfully processed</response>
        /// <response code="400">Error in the request parameters</response>
        [HttpGet("user")]
        public ActionResult<IEnumerable<IdentityUser>> Get([FromQuery] int pageNum = 0, [FromQuery] int pageSize = 10)
        {
            try
            {
                return Ok(_repo.GetUser(pageSize, pageNum));
            }
            catch (ArgumentOutOfRangeException e)
            {
                return BadRequest(e.Message);
            }
        }



    }
}
