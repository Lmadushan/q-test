using Microsoft.AspNetCore.Identity;

namespace QualitAppsTest.Infrastructure.Models
{
    public partial class ApplicationUser : IdentityUser
    {
        public string? CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}
