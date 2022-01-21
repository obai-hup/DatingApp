using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.Intrfaces
{
    public interface ITokkenService
    {
        string CreateToken(AppUser user);
    }
}