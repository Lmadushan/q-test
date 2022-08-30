using QualitAppsTest.Service.Contracts;
using QualitAppsTest.Infrastructure.Model;
using Microsoft.AspNetCore.Mvc;

namespace QualitAppsTest
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagementController : ControllerBase
    {
        private readonly IManagementService _managementService;

        public ManagementController(
            IManagementService managementService)
        {
            _managementService = managementService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var response = await _managementService.Register(model);
            return Ok(new Response { Status = response.Status, Message = response.Message });
        }
    }
}
