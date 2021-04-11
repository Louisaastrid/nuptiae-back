using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Catalog.API.Controllers
{
    /// <summary>
    /// Catalog controller.
    /// </summary>
    /// <seealso cref="ControllerBase"/>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly ILogger<CatalogController> _logger;
        private readonly ICatalogRepo _repo;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="repo">Instance of <see cref="ICatalogRepo"/>.</param>
        /// <param name="logger">Instance of <see cref="ILogger{CatalogController}"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/> is <c>Null</c>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="repo"/> is <c>Null</c>.</exception>
        public CatalogController(ICatalogRepo repo, ILogger<CatalogController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /// <summary>
        /// Gets a paginated list of the catalog destinations.
        /// </summary>
        /// <param name="pageNum">Page num.</param>
        /// <param name="pageSize">Page size.</param>
        /// <returns>Paginated list of Catalog Destinations.</returns>
        /// <response code="200">Request successfully processed</response>
        /// <response code="400">Error in the request parameters</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IReadOnlyCollection<CatalogTravel>>> GetAsync([FromQuery] int pageNum = 0, [FromQuery] int pageSize = 10)
        {
            try
            {
                return Ok(await _repo.GetTravelAsync(pageSize, pageNum).ConfigureAwait(false));
            }
            catch (ArgumentOutOfRangeException e)
            {
                // ça va fonctionner, mais traditionellement on va plutôt faire le test des inputs directement dans le controlleur
                // plutôt que de lever une exception dans les couches basses
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception on method {nameof(GetAsync)}.");
                throw;
            }
        }

        /// <summary>
        /// Gets the specified destination from its identifier.
        /// </summary>
        /// <param name="id">Destination identifier.</param>
        /// <returns>Catalog destination found.</returns>
        /// <response code="200">Catalog Item with the given ID found.</response>
        /// <response code="404">No catalog item with the given ID found.</response>
        /// <response code="400">Invalid identifier.</response>
        [HttpGet("travels/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CatalogTravel>> GetTravelByIdAsync([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            try
            {
                var result = await _repo.GetTravelByIdAsync(id);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception on method {nameof(GetTravelByIdAsync)}.");
                throw;
            }
        }

        /// <summary>
        /// Gets the first destination related to a specific country. 
        /// </summary>
        /// <param name="country">Country search term.</param>
        /// <returns>Catalog destination found.</returns>
        /// <response code="200">Catalog Item with the given country found</response>
        /// <response code="404">No catalog item with the given country found</response>
        /// <response code="400">Invalid country.</response>
        [HttpGet("travels/{country:regex(^[[a-zA-Z]])}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CatalogTravel>> GetFirstTravelByCountryAsync([FromRoute] string country)
        {
            if (string.IsNullOrWhiteSpace(country))
            {
                return BadRequest();
            }

            try
            {
                var result = await _repo.GetFirstTravelByCountryAsync(country);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception on method {nameof(GetFirstTravelByCountryAsync)}.");
                throw;
            }
        }

        /// <summary>
        /// Deletes a destination with given id.
        /// </summary>
        /// <param name="id">Destination identifier.</param>
        /// <returns>Nothing.</returns>
        /// <response code="204">Success no content.</response>
        /// <response code="404">No catalog item with the given ID found.</response>
        /// <response code="400">Invalid identifier.</response>
        [HttpDelete("travels/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteTravelByIdAsync([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            try
            {
                var result = await _repo.GetTravelByIdAsync(id);
                if (result == null)
                {
                    return NotFound("Unknow travel.");
                }

                await _repo.RemoveTravelAsync(id).ConfigureAwait(false);
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception on method {nameof(DeleteTravelByIdAsync)}.");
                throw;
            }
        }

        /// <summary>
        /// Searches travels according to criteria request. 
        /// </summary>
        /// <param name="search">Search request.</param>
        /// <param name="pageNum">Page num.</param>
        /// <param name="pageSize">Page size.</param>
        /// <returns>Collection of destinations.</returns>
        /// <response code="200">Catalog items matching criteria.</response>
        /// <response code="400">Invalid request or pagination.</response>
        [HttpPost("travels/search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IReadOnlyCollection<CatalogTravel>>> GetTravelBySearchAsync([FromBody] CatalogTravelSearch search, [FromQuery] int pageNum = 0, [FromQuery] int pageSize = 10)
        {
            // TODO: ok là j'avoue sur cette méthode j'ai changé le paradigme...


            if (search == null)
            {
                return BadRequest();
            }

            try
            {
                return Ok(await _repo.FindTravelsByCountryAsync(search.Country, pageSize, pageNum).ConfigureAwait(false));
            }
            catch (ArgumentOutOfRangeException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception on method {nameof(GetTravelBySearchAsync)}.");
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newTravel"></param>
        /// <returns></returns>
        [HttpPost("travels")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> InsertTravelAsync([FromBody] CreateTravel newTravel)
        {
            if (newTravel == null)
            {
                return BadRequest();
            }

            try
            {
                var id = await _repo.AddNewTravelAsync(newTravel).ConfigureAwait(false);
                if (!id.HasValue)
                {
                    return Problem("Error");
                }
                return Created("travels/{id}", id.Value);
            }
            catch (ArgumentOutOfRangeException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception on method {nameof(InsertTravelAsync)}.");
                throw;
            }

        }
    }
}
