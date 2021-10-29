using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Data.Entities;

namespace Application.Interfaces
{
    public interface IJwtService
    {
        Task<string> GenerateToken(ApplicationUser user);
    }
}