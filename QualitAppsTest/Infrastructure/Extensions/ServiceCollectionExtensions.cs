using QualitAppsTest.Service;
using QualitAppsTest.Service.Contracts;

namespace QualitAppsTest.Middlewares;
public static class ServiceCollectionExtensions
{
    public static void AddCustomServices(this IServiceCollection services)
    {
        _ = services.AddScoped<IAuthenticateService, AuthenticationService>();
        _ = services.AddScoped<IManagementService, ManagementService>();
    }
}
