using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tazkartk.DTO;
using Tazkartk.Helpers;
using Tazkartk.Models;
using Tazkartk.Models.Enums;

namespace Tazkartk.Services
{
    public class AuthService:IAuthService
    {
        private readonly UserManager<User> _UserManager;
        private readonly JWT _jwt;
        public AuthService(UserManager<User> UserManager, IOptions<JWT> jwt)
        {
            _UserManager = UserManager;
            _jwt = jwt.Value;
        }

        public async Task<AuthModel> LoginAsync(LoginDTO DTO)
        {
            var authModel = new AuthModel();

            var user = await _UserManager.FindByEmailAsync(DTO.Email);

            if (user is null || !await _UserManager.CheckPasswordAsync(user, DTO.Password))
            {
                authModel.message = "Email or Password is incorrect!";
                return authModel;
            }

            var jwtSecurityToken = await CreateJwtToken(user);

            authModel.isAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            return authModel;
        }
        public async Task<AuthModel> RegisterAsync(RegisterDTO DTO)
        {
            if (await _UserManager.FindByEmailAsync(DTO.Email) != null)
            {
                return new AuthModel { message = "Email is already Registered" };
            }
            var user = new User
            {
                FirstName = DTO.FirstName,
                LastName = DTO.LastName,
                Email = DTO.Email,
                PhoneNumber = DTO.PhoneNumber,
                UserName = DTO.Email,

            };
            var result = await _UserManager.CreateAsync(user, DTO.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description} , ";
                }
                return new AuthModel { message = errors };
            }
            await _UserManager.AddToRoleAsync(user, Roles.User.ToString());
            var JwtSecurityToken = await CreateJwtToken(user);
            return new AuthModel
            {
                Email = DTO.Email,
                ExpiresOn = JwtSecurityToken.ValidTo,
                isAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(JwtSecurityToken),
            };
        }
        private async Task<JwtSecurityToken> CreateJwtToken(User user)
        {
            var userClaims = await _UserManager.GetClaimsAsync(user);
            var roles = await _UserManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid",user.Id.ToString()),
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));

            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwt.DurationInMinutes),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
    }
}
