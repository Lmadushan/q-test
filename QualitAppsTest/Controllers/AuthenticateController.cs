using QualitAppsTest.Service.Contracts;
using QualitAppsTest.Infrastructure.Model;
using Microsoft.AspNetCore.Mvc;

namespace QualitAppsTest
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly IAuthenticateService _authService;

        public AuthenticateController(
            IAuthenticateService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var token = await _authService.GenerateToken(model);
            if (token != null)
            {
                return Ok(token);
            }
            return Unauthorized();
        }
    }
}
