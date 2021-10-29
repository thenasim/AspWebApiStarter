using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.Config;
using Application.Interfaces;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Application.Services
{
    public class JwtService : IJwtService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtConfig _jwtConfig;

        public JwtService(UserManager<ApplicationUser> userManager, JwtConfig jwtConfig)
        {
            _userManager = userManager;
            _jwtConfig = jwtConfig;
        }
        
        public async Task<string> GenerateToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            
            var roleClaims = roles
                .Select(role => new Claim("roles", role))
                .ToList();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
                .Union(userClaims)
                .Union(roleClaims);
            
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtConfig.DurationInMinutes),
                signingCredentials: signingCredentials);
            
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            
            return token;
        }
    }
}