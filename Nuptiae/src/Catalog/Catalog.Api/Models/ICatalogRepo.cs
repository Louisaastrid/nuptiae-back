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
        /// Gets information about all travels.
        /// </summary>
        /// <param name="pageSize">Pagination size.</param>
        /// <param name="pageNum">Pagination number (starts at <c>0</c>).</param>
        /// <returns>All travels.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="pageSize"/> is below <c>1</c> or above <c>50</c>.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="pageNum"/> is below <c>0</c>.</exception>
        Task<IEnumerable<CatalogTravel>> GetTravel(int pageSize, int pageNum);

        /// <summary>
        /// Gets a single travel by its identifier.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<CatalogTravel> GetTravelById(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        Task<CatalogTravel> GetFirstTravelByCountry(string search);


        void RemoveTavel(int id);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        Task<IEnumerable<CatalogTravel>> FindTravelsByCountry(string search, int pageSize, int pageNum);

        ///// <summary>
        ///// Add a new Travel 
        ///// </summary>
        ///// <param name="newTravel"></param>
        ///// <returns>A new travel added </returns>
        //CatalogTravel AddNewTravel(CatalogTravel newTravel);

    }
}
