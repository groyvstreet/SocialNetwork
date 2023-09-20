using IdentityService.BLL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.PL.Extensions
{
    public static class JwtAuthenticationExtensions
    {
        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection("JwtOptions"));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOptions>()!;

                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = jwtOptions.GetSymmetricSecurityKey()
                };
            });
        }
    }
}
