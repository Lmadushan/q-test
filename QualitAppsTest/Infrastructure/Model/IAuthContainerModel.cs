using System.Security.Claims;

namespace QualitAppsTest.Infrastructure.Model
{
    public interface IAuthContainerModel
    {
        string UserId { get; set; }
        string SecretKey { get; set; }
        string SecurityAlgorithm { get; set; }
        int ExpireMinutes { get; set; }
        int ExpireMinutesRefreshToken { get; set; }
        Claim[] Claims { get; set; }
    }
}
