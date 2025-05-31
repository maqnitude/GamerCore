using GamerCore.Core.Constants;
using GamerCore.Core.Entities;
using GamerCore.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GamerCore.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UsersController> _logger;

        public UsersController(UserManager<User> userManager, ILogger<UsersController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [Authorize(Roles = RoleConstants.Admin)]
        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetUsers()
        {
            // TODO: Add pagination
            var users = _userManager.Users.ToList();

            if (users == null || users.Count == 0)
            {
                return NoContent();
            }

            var userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains(RoleConstants.Admin))
                {
                    continue;
                }

                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = [.. roles]
                });
            }

            _logger.LogInformation("Successfully retrieved {Count} users.", userDtos.Count);
            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound(ApiResponse<object>.CreateError(
                    StatusCodes.Status404NotFound,
                    "User not found."));
            }

            return Ok(ApiResponse<UserDto>.CreateSuccess(new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName
            }));
        }
    }
}