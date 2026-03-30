using Confessly.Configuration;
using Confessly.Domain;
using Confessly.Domain.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Confessly.Infrastructure.Authentication
{
    public class ConfesslyAuthentication : IConfesslyAuthentication
    {
        private readonly PasswordHasher<IUser> _passwordHasher;
        public ConfesslyAuthentication()
        {
            _passwordHasher = new PasswordHasher<IUser>();
        }

        public string HashPassword(User user)
        {
            return _passwordHasher.HashPassword(user, user.Password);
        }

        public bool VerifyPassword(User user, string passwordToVerify)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, passwordToVerify);
            return result == PasswordVerificationResult.Success;
        }

        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // Get JWT settings from configuration
            var jwtSecret = ConfesslyConfiguration.JWTSecret ?? throw new InvalidOperationException("JWT Secret is not configured");
            var jwtIssuer = ConfesslyConfiguration.JWTIssuer ?? throw new InvalidOperationException("JWT Issuer is not configured");
            var jwtAudience = ConfesslyConfiguration.JWTAudience ?? throw new InvalidOperationException("JWT Audience is not configured");
            var jwtExpirationMinutes = ConfesslyConfiguration.JWTExpirationMinutes;

            var key = Encoding.ASCII.GetBytes(jwtSecret);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Name),
                new("jti", Guid.NewGuid().ToString()),
                new("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            // TODO: Add for roles and other claims as needed

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(jwtExpirationMinutes),
                Issuer = jwtIssuer,
                Audience = jwtAudience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
