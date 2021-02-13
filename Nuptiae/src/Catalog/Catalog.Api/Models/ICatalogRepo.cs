using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Api.Models
{
    public interface ICatalogRepo : IDisposable
    {

        /// <summary>
        /// Get travels information
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNum"></param>
        /// <returns>All travels</returns>
        Task<IEnumerable<CatalogTravel>> GetTravel(int pageSize, int pageNum);
        /// <summary>
        /// Get travels by id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>one travels</returns>
        Task<CatalogTravel> GetTravelById(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        Task<CatalogTravel> FindTravelByCountry(string search);


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
