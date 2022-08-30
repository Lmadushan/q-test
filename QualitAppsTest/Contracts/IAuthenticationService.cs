using QualitAppsTest.Service.DTO.Response;
using QualitAppsTest.Infrastructure.Model;
using QualitAppsTest.Infrastructure.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace QualitAppsTest.Service.Contracts;
public interface IAuthenticateService
{
    #region Methods
    JwtSecurityToken GetToken(List<Claim> authClaims);
    Task<ApplicationUser?> ValidateCredentials(LoginModel model);
    Task<JWTResponseDTO?> GenerateToken(LoginModel model);
    #endregion
}
