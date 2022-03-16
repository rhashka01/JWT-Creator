using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JWT_Creator
{
    public class JwtHelper
    {
        public static string GenerateJwtTokenWithSymmetricKey_HMACSHA256(string key, string issuer, string audience, IEnumerable<Claim> claims)
        {
            var secretKey = Encoding.ASCII.GetBytes(key);

            var signingKey = new SymmetricSecurityKey(secretKey);

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public static bool ValidateJwtTokenWithSymmetricKey_HMACSHA256(string token, string key, string issuer, string audience)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = securityKey  
                }, out SecurityToken validatedToken);

                return (validatedToken != null);

            } catch (Exception ex)
            {
                // logging goes in here.
                return false;
            }
        }

        public static string GenerateJwtTokenWithAsymmetricKey_RSA256(RsaSecurityKey key, string issuer, string audience, IEnumerable<Claim> claims, DateTime? expires = null)
        {
            if(expires == null)
            {
                expires = DateTime.UtcNow.AddDays(7);
            }

            var tokenDescriptor = new SecurityTokenDescriptor() {
                Issuer = issuer,
                Audience = audience,
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256Signature)
            };

            var jwtHandler = new JwtSecurityTokenHandler();

            var token = jwtHandler.CreateToken(tokenDescriptor);

            return jwtHandler.WriteToken(token);
        }

        public static bool ValidateJwtTokenWithAsymmetricKey_RSA256(string token, RsaSecurityKey key, string issuer, string audience, out string message)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters() { 
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true, 
                    ValidateAudience = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = key
                }, out SecurityToken validatedToken);

                message = $"valid Token: [ {token} ]";

                return (validatedToken != null);

            } catch(Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }
    }
}
