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
        IEnumerable<CatalogTravel> GetTravel(int pageSize, int pageNum);
        /// <summary>
        /// Get travels by id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>one travels</returns>
        CatalogTravel GetTravelById(int id);

    }
}
