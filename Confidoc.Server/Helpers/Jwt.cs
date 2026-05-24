using Confidoc.Server.Models;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Confidoc.Server.Helpers
{
    public static class Jwt
    {
        public static string? GetUsernameFromToken(string token)
        {
            var secretKey = GetSecretKey();
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var validationParameters = new TokenValidationParameters()
            {
                ValidateLifetime = false, 
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidIssuer = Configuration.GetEnvVariable("CONFIDOC_JWT_ISSUER"),
                ValidAudience = Configuration.GetEnvVariable("CONFIDOC_JWT_AUDIENCE"),
                IssuerSigningKey = secretKey
            };
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            handler.ValidateToken(token, validationParameters, out var validToken);
            return jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        }

        public static string GenerateToken(ConfidocUser user)
        {
            if (user is null || user.UserName is null)
                throw new ArgumentNullException(nameof(user));

            var secretKey = GetSecretKey();

            var claims = new List<Claim>()
            {
                new(ClaimTypes.Name, user.UserName),
            };

            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokenOptions = new JwtSecurityToken(
                issuer: Configuration.GetEnvVariable("CONFIDOC_JWT_ISSUER"),
                audience: Configuration.GetEnvVariable("CONFIDOC_JWT_AUDIENCE"),
                claims: claims,
                expires: DateTime.Now.AddMinutes(int.Parse(Configuration.GetEnvVariable("CONFIDOC_JWT_EXPIRES"))),
                signingCredentials: signinCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        public static SymmetricSecurityKey GetSecretKey()
        {
            return new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    Configuration.GetEnvVariable("CONFIDOC_JWT_SECRET")
                )
            );
        }
    }
}
