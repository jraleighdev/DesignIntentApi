using Domain.Models.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
