using QualitAppsTest.Infrastructure.Model;
using QualitAppsTest.Infrastructure.Models;

namespace QualitAppsTest.Service.Contracts;
public interface IBookingService
{
    #region Methods
    Task<Response> Create(Booking model);
    Task<Response> GetAll(Booking model);
    Task<Response> Update(Booking model);
    Task<Response> Remove(Booking model);
    #endregion
}
