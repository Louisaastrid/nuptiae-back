using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Api.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class IdentityUser
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id_User { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Surname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ZipCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Adress { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }
    }
}
