using QualitAppsTest.Infrastructure.Model;
using System.Security.Claims;

namespace QualitAppsTest.Infrastructure.Security
{
    public interface IAuthService
    {
        string SecretKey { get; set; }

        bool IsTokenValid(string token);
        string GenerateToken(JWTContainerModel model);
        string GenerateRefreshToken(JWTContainerModel model);
        IEnumerable<Claim> GetTokenClaims(string token);
    }
}
