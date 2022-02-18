using System;
using API.Interface;
using API.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.Service
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey key;

        public TokenService(IConfiguration config)
        {
            key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }

        public string CreateToken(AppUser appUser)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
	        {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, appUser.UserName),
                }),
                Expires = DateTime.UtcNow.AddDays(7),       
                Issuer = "https://localhost:5001", 
                Audience =  "https://localhost:5001",      
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
	        };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}