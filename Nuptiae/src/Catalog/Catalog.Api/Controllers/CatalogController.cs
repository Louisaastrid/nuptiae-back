using Catalog.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly ILogger<CatalogController> _logger;
        private readonly ICatalogRepo _repo;

        //public CatalogController ():this(new ICatalogRepo(),new ILogger<CatalogController>) { }
        public CatalogController(ICatalogRepo repo, ILogger<CatalogController> logger)
        {
            _logger = logger;
            _repo = repo;
        }

        /// <summary>
        /// Get a paginated list of the catalog destinations
        /// </summary>
        /// <param name="pageNum"> Page Num</param>
        /// <param name="pageSize">Page Size </param>
        /// <returns>Paginated list of Catalog Destinations </returns>
        /// <response code="200">Request successfully processed</response>
        /// <response code="400">Error in the request parameters</response>
        [HttpGet("travel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<CatalogTravel>>> Get([FromQuery] int pageNum = 0, [FromQuery] int pageSize = 10)
        {
            try
            {
                return Ok(await _repo.GetTravel(pageSize, pageNum));
            }
            catch (ArgumentOutOfRangeException e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// get the specified destination from his id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Catalog Destnations found </returns>
        /// <response code="200">Catalog Item with the given ID found</response>
        /// <response code="404">No catalog item with the given ID found</response>
        [HttpGet("travel/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CatalogTravel>> Get(int id)
        {
            var result = await _repo.GetTravelById(id);
            if (result == null)
            {
                return NotFound();
            }
            else
            {
                return result;
            }

        }


        /// <summary>
        /// get the specified destination from his country 
        /// </summary>
        /// <param name="search"></param>
        /// <returns>Catalog Destnations found </returns>
        /// <response code="200">Catalog Item with the given ID found</response>
        /// <response code="404">No catalog item with the given ID found</response>
        [HttpGet("travel/{search}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CatalogTravel>> Get(string search)
        {
            var result = await _repo.FindTravelByCountry(search);
            if (result == null)
            {
                return NotFound();
            }
            return result;

        }

        /// <summary>
        /// delete a destination with given id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>" "</returns>
        /// <response code="200">Catalog Item with the given ID found</response>
        /// <response code="204">Success no content</response>
        /// <response code="404">No catalog item with the given ID found</response>
        [HttpDelete("travel/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            CatalogTravel result = await _repo.GetTravelById(id);
            if (result == null)
            {
                return NotFound("unKnow travel");
            }
            _repo.RemoveTavel(id);
            return NoContent();
        }

        /// <summary>
        /// Search with Country return all country travel by name 
        /// </summary>
        /// <param name="country"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("trip/{search:regex(^[[a-zA-Z]])}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<CatalogTravel>>> Get([FromQuery] string search, [FromQuery] int pageNum = 0, [FromQuery] int pageSize = 10)
        {
            try
            {
                return Ok(await _repo.FindTravelsByCountry(search, pageSize, pageNum));
            }
            catch (ArgumentOutOfRangeException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
