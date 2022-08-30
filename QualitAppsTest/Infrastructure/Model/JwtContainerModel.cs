using System.Security.Claims;
namespace QualitAppsTest.Infrastructure.Model
{
    public class JWTContainerModel : IAuthContainerModel
    {
        public bool UseJwt { get; set; }
        public string UserId
        {
            get => UserId;
            set => Claims = new Claim[] { new Claim(ClaimTypes.Name, value) };//this.UserId = value;
        }
        public string SecretKey { get; set; }
        public string SecurityAlgorithm { get; set; }
        public int ExpireMinutes { get; set; }
        public int ExpireMinutesRefreshToken { get; set; }
        public Claim[] Claims { get; set; }
    }
}
