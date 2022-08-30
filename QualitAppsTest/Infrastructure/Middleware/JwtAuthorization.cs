using QualitAppsTest.Infrastructure.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Principal;

namespace QualitAppsTest.Infrastructure.Middleware
{
    public class IJwtAuthorization : IAuthorizationRequirement
    {
        public JWTContainerModel jwtSettings;
        public IJwtAuthorization(JWTContainerModel jwtSettings)
        {
            this.jwtSettings = jwtSettings;
        }
    }

    public class JwtAuthorization : AuthorizationHandler<IJwtAuthorization>, IAuthorizationRequirement
    {
        IHttpContextAccessor _httpContextAccessor = null;
        JWTContainerModel _jwtSettings;

        public JwtAuthorization(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IJwtAuthorization requirement)
        {
            foreach (IAuthorizationRequirement element in context.PendingRequirements)
            {
                if (element is IJwtAuthorization)
                {
                    _jwtSettings = ((IJwtAuthorization)element).jwtSettings;
                    break;
                }
            }

            //bool authenticationPassed = false;
            //if (context.PendingRequirements.Count() == 0)
            //{
            //    context.Succeed(requirement);
            //    authenticationPassed = true;
            //}

            bool hasAuthorization = _httpContextAccessor.HttpContext.Request.Headers.ContainsKey("Authorization");
            Console.WriteLine("hasAuthorization: " + hasAuthorization);

            //Automatically accept connection if UseJwt is false
            if (context.PendingRequirements.Count() == 0 || !_jwtSettings.UseJwt)
            {
                context.Succeed(requirement);
            }
            //check authorization
            else if (hasAuthorization)
            {
                StringValues tokenKey;
                if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out tokenKey))
                {
                    int startIndex = tokenKey[0].LastIndexOf(' ') + 1;
                    int length = tokenKey[0].Length - startIndex;
                    string tokenString = tokenKey[0].Substring(startIndex, length);

                    if (ValidateToken(tokenString))
                    {
                        context.Succeed(requirement);
                    }
                    else
                    {
                        context.Fail();
                    }
                }
                else //shouldnt come here since we already checked earlier that Authorization exists
                {
                    context.Fail();
                }
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }

        private bool ValidateToken(string authToken)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters validationParameters = GetValidationParameters();

            SecurityToken validatedToken;
            try
            {
                IPrincipal principal = tokenHandler.ValidateToken(authToken, validationParameters, out validatedToken);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //todo: refactor this as similar item already exists inside JwtAdapter class.
        private TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = false,
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(_jwtSettings.SecretKey)), //convert from base64 string else key validation error.
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,            //validate the token lifetime
                ClockSkew = TimeSpan.FromMinutes(1) //allow difference of 1 mins expiry
            };
        }
    }
}