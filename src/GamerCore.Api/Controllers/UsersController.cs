using GamerCore.Core.Constants;
using GamerCore.Core.Entities;
using GamerCore.Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GamerCore.Api.Controllers
{
    [Authorize(
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
        Roles = RoleNames.Administrator)]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<UsersController> _logger;

        public UsersController(UserManager<AppUser> userManager, ILogger<UsersController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<AppUserDto>>> GetUsersAsync()
        {
            // TODO: Add pagination
            var users = _userManager.Users.ToList();

            if (users == null || users.Count == 0)
            {
                return NoContent();
            }

            var userDtos = new List<AppUserDto>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains(RoleNames.Administrator))
                {
                    continue;
                }

                userDtos.Add(new AppUserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = roles.ToList()
                });
            }

            _logger.LogInformation("Successfully retrieved {Count} users.", userDtos.Count);
            return Ok(userDtos);
        }
    }
}