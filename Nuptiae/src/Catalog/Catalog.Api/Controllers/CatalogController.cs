using Catalog.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CatalogController : ControllerBase
    {

        private readonly ILogger<CatalogController> _logger;
        private readonly ICatalogRepo _repo;

        public CatalogController(ICatalogRepo repo, ILogger<CatalogController> logger)
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
        [HttpGet("travel")]
        public ActionResult<IEnumerable<CatalogTravel>> Get([FromQuery] int pageNum = 0, [FromQuery] int pageSize = 10)
        {
            try
            {
                return Ok(_repo.GetTravel(pageSize, pageNum));
            }
            catch (ArgumentOutOfRangeException e)
            {
                return BadRequest(e.Message);
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        /// <summary>
        /// get the specified destination from his id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Catalog Destnations found </returns>
        /// <response code="200">Catalog Item with the given ID found</response>
        /// <response code="404">No catalog item with the given ID found</response>
        [HttpGet("travel/{id:int}")]
        public ActionResult<CatalogTravel> Get(int id)
        {
            var result = _repo.GetTravelById(id);
            if (result == null)
            {
                return NotFound();
            }
            else
            {
                return result;
            }

        }
    }
}