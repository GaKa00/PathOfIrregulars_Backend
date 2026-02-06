using Microsoft.IdentityModel.Tokens;
using PathOfIrregulars.Infrastructure.Database.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PathOfIrregulars.API.Auth
{
    public class JwtTokenService
    {
        private readonly IConfiguration _config;

        public JwtTokenService(IConfiguration config)
        {
            _config = config;
        }

        public string CreateToken(Account account)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim(ClaimTypes.Name, account.Username)
            };

            var key = new SymmetricSecurityKey(
      Encoding.UTF8.GetBytes(_config["Jwt:Key"])
  );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
         issuer: _config["Jwt:Issuer"],
         audience: _config["Jwt:Audience"],
         claims: claims,
         expires: DateTime.UtcNow.AddHours(12),
         signingCredentials: creds
     );


            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
    }
