using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Api.Models
{
    public class CatalogTravel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Depature { get; set; }
        public decimal Price { get; set; }
        public string Town { get; set; }
        public string Country { get; set; }
       

    }
}
