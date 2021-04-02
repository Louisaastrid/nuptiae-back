using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Api.Models
{
    public interface IIdentityRepo
    {
        IEnumerable<IdentityUser> GetUser(int pageSize, int pageNum);
        IEnumerable<IdentityUser> NewUser(IdentityUser newUser);
        
    }
}
