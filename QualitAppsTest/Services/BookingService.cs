using QualitAppsTest.Service.Contracts;
using QualitAppsTest.Infrastructure.DAL;
using QualitAppsTest.Infrastructure.Model;
using QualitAppsTest.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace QualitAppsTest.Service;
public class BookingService : BaseDAL, IManagementService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public BookingService(RoleManager<IdentityRole> roleManager, IConfiguration configuration, ILogger<AuthenticationService> logger, UserManager<ApplicationUser> userManager) : base(configuration, logger)
    {
        _configuration = configuration;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    #region Methods
    public async Task<Response> Register(RegisterModel model)
    {
        var result = await ValidateRegisteration(model);

        if (result.Status != null)
        {
            return result;
        }

        // Create User
        ApplicationUser user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username
        };

        var response = await _userManager.CreateAsync(user, model.Password);

        if (!response.Succeeded)
        {
            result.Status = "Error";
            result.Message = "User creation failed! Please check user details and try again.";
            return result;
        }

        // Add user roles
        IdentityResult roleRes;
        if (model.Role == UserRoles.Admin)
        {
            IEnumerable<string> roles = new[] { UserRoles.Customer, UserRoles.Admin, UserRoles.Driver };
            roleRes = await _userManager.AddToRolesAsync(user, roles);
        }
        else
        {
            roleRes = await _userManager.AddToRoleAsync(user, model.Role);
        }

        if (!roleRes.Succeeded)
        {
            var createdUser = await _userManager.FindByNameAsync(model.Username);
            await _userManager.DeleteAsync(createdUser);

            result.Status = "Error";
            result.Message = "User creation failed when ading user roles! Please check user details and try again.";
            return result;
        }

        result.Status = "Success";
        result.Message = "User created successfully!";
        return result;
    }

    private async Task<RegistrationResponse?> ManageExistingUserRoles()
    {
        RegistrationResponse response = new();
        // Create relevent user roles if not exists
        if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
        if (!await _roleManager.RoleExistsAsync(UserRoles.Customer))
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.Customer));
        if (!await _roleManager.RoleExistsAsync(UserRoles.Driver))
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.Driver));

        // Check if relevent user roles created
        if (!await _roleManager.RoleExistsAsync(UserRoles.Admin) || !await _roleManager.RoleExistsAsync(UserRoles.Customer) || !await _roleManager.RoleExistsAsync(UserRoles.Driver))
        {
            response.Message = "Relavent User Roles Not found!";
            return response;
        }

        return null;
    }

    private async Task<RegistrationResponse> ValidateRegisteration(RegisterModel model)
    {
        RegistrationResponse response = new()
        {
            Status = "Error",
            user = null
        };
        // Check if User Exists
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user != null)
        {
            response.Message = "User already exists!";
            return response;
        }

        // Validate User roles
        if (!(UserRoles.Admin == model.Role || UserRoles.Customer == model.Role || UserRoles.Driver == model.Role))
        {
            response.Message = "Invalid Role found! Please check user details and try again.";
            return response;
        }

        // Manage Existing Roles
        var rolesRes = await ManageExistingUserRoles();
        if (rolesRes != null)
        {
            return rolesRes;
        }

        response.user = user;
        response.Status = null;
        return response;
    }

    #endregion
}
