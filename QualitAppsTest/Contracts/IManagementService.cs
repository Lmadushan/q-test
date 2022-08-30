using QualitAppsTest.Infrastructure.Model;

namespace QualitAppsTest.Service.Contracts;
public interface IManagementService
{
    #region Methods
    Task<Response> Register(RegisterModel model);
    #endregion
}
