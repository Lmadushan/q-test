using QualitAppsTest.Infrastructure.Models;

namespace QualitAppsTest.Infrastructure.Model
{
    public class RegistrationResponse : Response
    {
        public ApplicationUser? user { get; set; }
    }
}
