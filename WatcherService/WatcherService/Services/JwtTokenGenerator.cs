using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WatcherService.Services.IServices;

namespace WatcherService.Services
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        public string CreateToken(string iss, int time, string sharedSecret)
        {
            var exp = DateTime.UtcNow.AddMinutes(time);
            //var exp = now.AddMinutes(time);


            var tokenHndler = new JwtSecurityTokenHandler();
            var keyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(sharedSecret));

            var SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = iss,                 // iss claim
                Expires = exp,    // exp claim (בדיוק 5 דקות)
                SigningCredentials = SigningCredentials
            };

            var token = tokenHndler.CreateToken(tokenDescriptor);
            return tokenHndler.WriteToken(token);
        }
    }
}
