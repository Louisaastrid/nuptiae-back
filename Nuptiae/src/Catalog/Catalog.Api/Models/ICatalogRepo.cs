using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.Api.Models
{
    /// <summary>
    /// Travel catalog repository interface.
    /// </summary>
    public interface ICatalogRepo
    {
        /// <summary>
        /// Gets paginated information about all travels.
        /// </summary>
        /// <param name="pageSize">Pagination size.</param>
        /// <param name="pageNum">Pagination number (starts at <c>0</c>).</param>
        /// <returns>All travels.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="pageSize"/> is below <c>1</c> or above <c>50</c>.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="pageNum"/> is below <c>0</c>.</exception>
        Task<IReadOnlyCollection<CatalogTravel>> GetTravelAsync(int pageSize, int pageNum);

        /// <summary>
        /// Gets a single travel by its identifier.
        /// </summary>
        /// <param name="id">Travel identifier.</param>
        /// <returns>Travel instance or <c>Null</c>.</returns>
        Task<CatalogTravel> GetTravelByIdAsync(int id);

        /// <summary>
        /// Gets the first travel for the specified country.
        /// </summary>
        /// <param name="search">Country search.</param>
        /// <returns>Travel instance or <c>Null</c>.</returns>
        Task<CatalogTravel> GetFirstTravelByCountryAsync(string search);

        /// <summary>
        /// Removes a traval.
        /// </summary>
        /// <param name="id">Travel identifier.</param>
        /// <returns>Nothing.</returns>
        Task RemoveTavelAsync(int id);

        /// <summary>
        /// Gets paginated information about all travels for a specified country.
        /// </summary>
        /// <param name="search">Country search.</param>
        /// <param name="pageSize">Pagination size.</param>
        /// <param name="pageNum">Pagination number (starts at <c>0</c>).</param>
        /// <returns>All country travels.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="pageSize"/> is below <c>1</c> or above <c>50</c>.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="pageNum"/> is below <c>0</c>.</exception>
        Task<IReadOnlyCollection<CatalogTravel>> FindTravelsByCountryAsync(string search, int pageSize, int pageNum);

        /// <summary>
        /// Adds a new travel.
        /// </summary>
        /// <param name="newTravel">Travel information.</param>
        /// <returns>Travel identifier (<c>Null</c> if failure).</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="newTravel"/> is <c>Null</c>.</exception>
        Task<int?> AddNewTravelAsync(CatalogTravel newTravel);
    }
}
