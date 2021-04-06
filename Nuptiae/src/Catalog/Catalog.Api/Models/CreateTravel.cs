using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Api.Models
{
    /// <summary>
    /// CreateTravel DTO
    /// </summary>
    public class CreateTravel
    {
        /// <summary>
        /// Identifier.
        /// </summary>
        public int? Id { get; set; }
        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Departure date.
        /// </summary>
        public DateTime Departure { get; set; }

        /// <summary>
        /// Price.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Town.
        /// </summary>
        public string Town { get; set; }

        /// <summary>
        /// Country.
        /// </summary>
        public string Country { get; set; }
    }
}
