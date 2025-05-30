using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tazkartk.Application.Interfaces;
using Tazkartk.Domain.Models;
using Tazkartk.Infrastructure.Helpers;

namespace Tazkartk.Infrastructure.Token
{
    internal class TokenService : ITokenService
    {
        private readonly UserManager<Account> _AccountManager;
        private readonly JWT _jwt;

        public TokenService(UserManager<Account> accountManager, IOptions<JWT> jwt)
        {
            _AccountManager = accountManager;
            _jwt = jwt.Value;
        }

        public async Task<string> GenerateToken(Account account)
        {
            var AccountClaims = await _AccountManager.GetClaimsAsync(account);
            var roles = await _AccountManager.GetRolesAsync(account);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, account.Email),
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim(ClaimTypes.Email, account.Email) // 
                //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //new Claim(JwtRegisteredClaimNames.Email, account.Email),
                //new Claim("uid",account.Id.ToString()),
            }
            .Union(AccountClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var now = DateTime.UtcNow;
            var jwtSecurityToken = new JwtSecurityToken(
             issuer: _jwt.Issuer,
             audience: _jwt.Audience,
             claims: claims,
             expires: now.AddMinutes(_jwt.DurationInMinutes),
             signingCredentials: signingCredentials);
            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        }
    }
}
