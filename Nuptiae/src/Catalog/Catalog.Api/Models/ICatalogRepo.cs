using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Api.Models
{
    public interface ICatalogRepo : IDisposable
    {
        IEnumerable<CatalogTravel> GetTravel(int pageSize, int pageNum);
        CatalogTravel GetTravelById(int id);

    }
}
