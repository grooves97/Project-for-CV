using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OnlineShop.DataAccess;
using OnlineShop.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.Services
{
    public class AuthService
    {
        private readonly OnlineShopContext onlineShopContext;
        private readonly string jwtSecret;

        public AuthService(OnlineShopContext onlineShopContext, IOptions<JWTSecretOptions> secretOptions)
        {
            this.onlineShopContext = onlineShopContext;
            jwtSecret = secretOptions.Value.JWTSecret;
        }

        public async Task<string> Authenticate(string phoneNumber, string code)
        {
            var existingUser = await onlineShopContext.Users.SingleOrDefaultAsync(user => user.PhoneNumber==phoneNumber && user.VerificationCode==code);

            if (existingUser == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.MobilePhone, phoneNumber)
                }),

                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            return tokenHandler.WriteToken(token);
        }
    }
}
