
using QualitAppsTest.Infrastructure.Middleware;
using QualitAppsTest.Infrastructure.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Cardtrend.Core.WebAPI.Library.Infrastructure.Security
{
    public static class JwtService
    {
        public static IServiceCollection AddJwtService(this IServiceCollection service, JWTContainerModel jwtSettings)
        {
            if (jwtSettings.UseJwt)
            {

                var key = Base64UrlEncoder.DecodeBytes(jwtSettings.SecretKey); //ensure that the key is Base64URL decoded first else "The signature is invalid" error.
                service.AddAuthentication(config => config.DefaultScheme = JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(config =>
                    {
                        config.RequireHttpsMetadata = false;
                        config.SaveToken = true;
                        config.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = false,
                            IssuerSigningKey = new SymmetricSecurityKey(key),
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            ValidateLifetime = true,            //validate the token lifetime
                            ClockSkew = TimeSpan.FromMinutes(1) //allow difference of 1 mins expiry
                        };
                    });
            }
            service.AddHttpContextAccessor();
            service.AddAuthorization(options =>
            {
                options.AddPolicy("Jwt",
                    policy => policy.Requirements.Add(new IJwtAuthorization(jwtSettings)));
            });
            service.AddSingleton<IAuthorizationHandler, JwtAuthorization>();
            return service;
        }
    }
}
