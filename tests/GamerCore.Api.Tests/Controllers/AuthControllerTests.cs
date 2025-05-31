using System.Security.Claims;
using GamerCore.Api.Controllers;
using GamerCore.Api.Services;
using GamerCore.Core.Entities;
using GamerCore.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace GamerCore.Api.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;

        private readonly Mock<IUserStore<User>> _mockUserStore;
        private readonly Mock<UserManager<User>> _mockUserManager;

        private readonly Mock<ILogger<AuthController>> _mockLogger;

        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();

            _mockUserStore = new Mock<IUserStore<User>>();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            _mockUserManager = new Mock<UserManager<User>>(_mockUserStore.Object,
                null, null, null, null, null, null, null, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            _mockLogger = new Mock<ILogger<AuthController>>();

            _controller = new AuthController(_mockAuthService.Object, _mockUserManager.Object, _mockLogger.Object);
        }

        #region Register tests

        [Fact]
        public async Task Register_ReturnsCreated_WhenRegistrationSucceeds()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Email = "test@example.com",
                Password = "Password123!",
                FirstName = "Test",
                LastName = "User"
            };

            var user = new User
            {
                Id = "userId123",
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName
            };

            _mockAuthService.Setup(x => x.RegisterAsync(It.IsAny<RegisterDto>()))
                .ReturnsAsync(RegistrationResult.Success(user));

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var response = Assert.IsType<ApiResponse<UserDto>>(createdAtActionResult.Value);
            Assert.NotNull(response.Data);
            Assert.Equal(user.Id, response.Data.Id);
            Assert.Equal(user.Email, response.Data.Email);
            Assert.Equal(user.FirstName, response.Data.FirstName);
            Assert.Equal(user.LastName, response.Data.LastName);
            Assert.Equal("GetUserById", createdAtActionResult.ActionName);
            Assert.Equal("Users", createdAtActionResult.ControllerName);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var registerDto = new RegisterDto();
            _controller.ModelState.AddModelError("Email", "Email is required");

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.NotNull(response.Error);
            Assert.Equal(StatusCodes.Status400BadRequest, response.Error.Code);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenRegistrationFails()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Email = "test@example.com",
                Password = "Password123!",
                FirstName = "Test",
                LastName = "User"
            };

            const string errorMessage = "Email already in use.";
            _mockAuthService.Setup(x => x.RegisterAsync(It.IsAny<RegisterDto>()))
                .ReturnsAsync(RegistrationResult.Error(errorMessage));

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.NotNull(response.Error);
            Assert.Equal(StatusCodes.Status400BadRequest, response.Error.Code);
            Assert.Equal(errorMessage, response.Error.Message);
        }

        [Fact]
        public async Task Register_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Email = "test@example.com",
                Password = "Password123!",
                FirstName = "Test",
                LastName = "User"
            };

            _mockAuthService.Setup(x => x.RegisterAsync(It.IsAny<RegisterDto>()))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
            var response = Assert.IsType<ApiResponse<object>>(statusCodeResult.Value);
            Assert.NotNull(response.Error);
            Assert.Equal(StatusCodes.Status500InternalServerError, response.Error.Code);
        }

        #endregion

        #region Login tests

        [Fact]
        public async Task Login_ReturnsOk_WhenLoginSucceeds()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "Password123!",
                RememberMe = true
            };

            var user = new User
            {
                Id = "userId123",
                Email = loginDto.Email,
                FirstName = "Test",
                LastName = "User"
            };

            _mockAuthService.Setup(x => x.LoginAsync(It.IsAny<LoginDto>()))
                .ReturnsAsync(LoginResult.Success(null, user));

            var userRoles = new List<string> { "User" };
            _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(userRoles);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<UserDto>>(okResult.Value);
            Assert.NotNull(response.Data);
            Assert.Equal(user.Id, response.Data.Id);
            Assert.Equal(user.Email, response.Data.Email);
            Assert.Equal(user.FirstName, response.Data.FirstName);
            Assert.Equal(user.LastName, response.Data.LastName);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var loginDto = new LoginDto();
            _controller.ModelState.AddModelError("Email", "Email is required");

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.NotNull(response.Error);
            Assert.Equal(StatusCodes.Status400BadRequest, response.Error.Code);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenUserIsLockedOut()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "Password123!",
                RememberMe = false
            };

            _mockAuthService.Setup(x => x.LoginAsync(It.IsAny<LoginDto>()))
                .ReturnsAsync(LoginResult.LockedOut());

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.NotNull(response.Error);
            Assert.Equal(StatusCodes.Status400BadRequest, response.Error.Code);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenEmailOrPasswordIsInvalid()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "WrongPassword",
                RememberMe = false
            };

            _mockAuthService.Setup(x => x.LoginAsync(It.IsAny<LoginDto>()))
                .ReturnsAsync(LoginResult.InvalidCredentials());

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(unauthorizedResult.Value);
            Assert.NotNull(response.Error);
            Assert.Equal(StatusCodes.Status401Unauthorized, response.Error.Code);
        }

        [Fact]
        public async Task Login_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "Password123!",
                RememberMe = true
            };

            _mockAuthService.Setup(x => x.LoginAsync(It.IsAny<LoginDto>()))
                .ThrowsAsync(new Exception("Test Excpetion"));

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
            var response = Assert.IsType<ApiResponse<object>>(statusCodeResult.Value);
            Assert.NotNull(response.Error);
            Assert.Equal(StatusCodes.Status500InternalServerError, response.Error.Code);
        }

        #endregion

        #region LoginJwt tests

        [Fact]
        public async Task LoginJwt_ReturnsOk_WhenLoginSucceeds()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "admin@example.com",
                Password = "Password123!",
                RememberMe = true
            };

            const string jwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";
            _mockAuthService.Setup(x => x.LoginJwtAsync(It.IsAny<LoginDto>()))
                .ReturnsAsync(LoginResult.Success(jwtToken));

            // Act
            var result = await _controller.LoginJwt(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.Equal(jwtToken, response.Data);
        }

        [Fact]
        public async Task LoginJwt_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var loginDto = new LoginDto();
            _controller.ModelState.AddModelError("Email", "Email is required");

            // Act
            var result = await _controller.LoginJwt(loginDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.NotNull(response.Error);
            Assert.Equal(StatusCodes.Status400BadRequest, response.Error.Code);
        }

        [Fact]
        public async Task LoginJwt_ReturnsBadRequest_WhenUserIsLockedOut()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "admin@example.com",
                Password = "Password123!",
                RememberMe = false
            };

            _mockAuthService.Setup(x => x.LoginJwtAsync(It.IsAny<LoginDto>()))
                .ReturnsAsync(LoginResult.LockedOut());

            // Act
            var result = await _controller.LoginJwt(loginDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.NotNull(response.Error);
            Assert.Equal(StatusCodes.Status400BadRequest, response.Error.Code);
        }

        [Fact]
        public async Task LoginJwt_ReturnsForbidden_WhenAccessIsDenied()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                // Non-admin user
                Email = "user@example.com",
                Password = "Password123!",
                RememberMe = false
            };

            _mockAuthService.Setup(x => x.LoginJwtAsync(It.IsAny<LoginDto>()))
                .ReturnsAsync(LoginResult.AccessDenied());

            // Act
            var result = await _controller.LoginJwt(loginDto);

            // Assert
            var forbiddenResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status403Forbidden, forbiddenResult.StatusCode);
            var response = Assert.IsType<ApiResponse<object>>(forbiddenResult.Value);
            Assert.NotNull(response.Error);
            Assert.Equal(StatusCodes.Status403Forbidden, response.Error.Code);
        }

        [Fact]
        public async Task LoginJwt_ReturnsUnauthorized_WhenEmailOrPasswordIsInvalid()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "admin@example.com",
                Password = "WrongPassword",
                RememberMe = false
            };

            _mockAuthService.Setup(x => x.LoginJwtAsync(It.IsAny<LoginDto>()))
                .ReturnsAsync(LoginResult.InvalidCredentials());

            // Act
            var result = await _controller.LoginJwt(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(unauthorizedResult.Value);
            Assert.NotNull(response.Error);
            Assert.Equal(StatusCodes.Status401Unauthorized, response.Error.Code);
        }

        [Fact]
        public async Task LoginJwt_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "admin@example.com",
                Password = "Password123!",
                RememberMe = true
            };

            _mockAuthService.Setup(x => x.LoginJwtAsync(It.IsAny<LoginDto>()))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.LoginJwt(loginDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
            var response = Assert.IsType<ApiResponse<object>>(statusCodeResult.Value);
            Assert.NotNull(response.Error);
            Assert.Equal(StatusCodes.Status500InternalServerError, response.Error.Code);
        }

        #endregion

        #region Logout tests

        [Fact]
        public async Task Logout_ReturnsOk_WhenLogoutSucceeds()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, "userId123"),
                new(ClaimTypes.Email, "test@example.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockAuthService.Setup(x => x.LogoutAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Logout();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.Null(response.Data);
            Assert.Null(response.Error);
        }

        [Fact]
        public async Task Logout_ReturnsBadRequest_WhenLogoutFails()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, "userId123"),
                new(ClaimTypes.Email, "test@example.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockAuthService.Setup(x => x.LogoutAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Logout();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.NotNull(response.Error);
            Assert.Equal(StatusCodes.Status400BadRequest, response.Error.Code);
        }

        [Fact]
        public async Task LogoutAsync_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, "userId123"),
                new(ClaimTypes.Email, "test@example.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockAuthService.Setup(x => x.LogoutAsync(It.IsAny<ClaimsPrincipal>()))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _controller.Logout();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
            var response = Assert.IsType<ApiResponse<object>>(statusCodeResult.Value);
            Assert.NotNull(response.Error);
            Assert.Equal(StatusCodes.Status500InternalServerError, response.Error.Code);
        }

        #endregion
    }
}