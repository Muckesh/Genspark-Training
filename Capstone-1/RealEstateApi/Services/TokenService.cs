using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using RealEstateApi.Interaces;
using RealEstateApi.Models;

namespace RealEstateApi.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _symmetricSecurityKey;
        public TokenService(IConfiguration configuration)
        {
            _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Keys:JwtTokenKey"]));
        }
        public async Task<string> GenerateToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Role,user.Role)
            };

            var creds = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(2),
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            // var token = new JwtSecurityToken(
            //     claims:claims,
            //     expires: DateTime.UtcNow.AddDays(1),
            //     signingCredentials:creds
            // );

            // return new JwtSecurityTokenHandler().WriteToken(token);
            return tokenHandler.WriteToken(token);
        }
    }
}