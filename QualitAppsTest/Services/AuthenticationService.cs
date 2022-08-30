using QualitAppsTest.Service.Contracts;
using QualitAppsTest.Service.DTO.Response;
using QualitAppsTest.Infrastructure.DAL;
using QualitAppsTest.Infrastructure.Model;
using QualitAppsTest.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QualitAppsTest.Service;
public class AuthenticationService : BaseDAL, IAuthenticateService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthenticationService(IConfiguration configuration, ILogger<AuthenticationService> logger, UserManager<ApplicationUser> userManager) : base(configuration, logger)
    {
        _configuration = configuration;
        _userManager = userManager;
    }

    #region Account General

    public JwtSecurityToken GetToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );
        return token;
    }

    public async Task<ApplicationUser?> ValidateCredentials(LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            return user;
        }
        return null;
    }

    public async Task<JWTResponseDTO?> GenerateToken(LoginModel model)
    {
        var user = await ValidateCredentials(model);
        if (user != null)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var jwt = GetToken(authClaims);

            JWTResponseDTO result = new()
            {
                token = new JwtSecurityTokenHandler().WriteToken(jwt),
                expiration = jwt.ValidTo
            };

            return result;
        }
        return null;
    }
    #endregion
}
