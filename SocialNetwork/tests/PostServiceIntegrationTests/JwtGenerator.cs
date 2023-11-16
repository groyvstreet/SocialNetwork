using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PostServiceIntegrationTests
{
    public static class JwtGenerator
    {
        public static string GenerateToken(IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken(
                issuer: "authServer",
                audience: "authClient",
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(60)),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes("secret_key_@123456789+-")), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
