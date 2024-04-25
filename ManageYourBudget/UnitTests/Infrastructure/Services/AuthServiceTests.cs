using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Moq;
using System.Security.Claims;
using Domain.Entities;
using Persistence.AuthService;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Application.DTOs.AccountDTOs;

namespace UnitTests.Infrastructure.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<User>> _userManager;
        private readonly Mock<IEmailSender> _emailSender;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
        private readonly Mock<IAuthenticationService> _authService; // Ensure this is declared
        private HttpContext _mockHttpContext; // Ensure this is declared
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _userManager = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);
            _emailSender = new Mock<IEmailSender>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();

            // Initialize HttpContext and AuthenticationService
            _mockHttpContext = new DefaultHttpContext();
            _authService = new Mock<IAuthenticationService>();
            _httpContextAccessor.Setup(_ => _.HttpContext).Returns(_mockHttpContext);
            _mockHttpContext.RequestServices = new ServiceCollection()
                .AddSingleton(_authService.Object)
                .BuildServiceProvider();

            _service = new AuthService(_userManager.Object, _emailSender.Object, _httpContextAccessor.Object);
        }

        [Fact]
        public async Task AuthenticateUserAsync_ValidCredentials_ReturnsUser()
        {
            // Arrange
            var user = new User { Email = "user@example.com" };
            _userManager.Setup(x => x.FindByEmailAsync("user@example.com")).ReturnsAsync(user);
            _userManager.Setup(x => x.CheckPasswordAsync(user, "correctpasswoRD123")).ReturnsAsync(true);
            _authService.Setup(x =>
                x.SignInAsync(
                    It.IsAny<HttpContext>(),
                    It.IsAny<string>(),
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<AuthenticationProperties>()
                )
            ).Returns(Task.CompletedTask).Verifiable("SignIn was not called.");

            // Act
            var result = await _service.AuthenticateUserAsync(new UserLoginDTO { Email = "user@example.com", Password = "correctpasswoRD123" });

            // Assert
            Assert.NotNull(result);
            Assert.Equal("user@example.com", result.Email);
        }

        [Fact]
        public async Task AuthenticateUserAsync_InValidCredentials_FailsAuthentication()
        {
            // Arrange
            var user = new User { Email = "user@example.com" };
            _userManager.Setup(x => x.FindByEmailAsync("user@example.com")).ReturnsAsync(user);
            _userManager.Setup(x => x.CheckPasswordAsync(user, "correctpasswoRD123")).ReturnsAsync(false);
            _authService.Setup(x =>
                x.SignInAsync(
                    It.IsAny<HttpContext>(),
                    It.IsAny<string>(),
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<AuthenticationProperties>()
                )
            ).Returns(Task.CompletedTask).Verifiable("SignIn was not called.");

            // Act
            var result = await _service.AuthenticateUserAsync(new UserLoginDTO { Email = "user@example.com", Password = "wrongpassword" });

            // Assert
            Assert.Null(result);

            _authService.Verify(
                x => x.SignInAsync(
                    It.IsAny<HttpContext>(),
                    It.IsAny<string>(),
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<AuthenticationProperties>()
                ),
                Times.Never
            );
        }

        [Fact]
        public async Task RegisterUserAsync_ValidCredentials_SuccessfulRegistration()
        {
            // Arrange
            var userRegistrationDTO = new UserRegistrationDTO
            {
                UserName = "newuser",
                Email = "newuser@example.com",
                Password = "Password123!"
            };

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = userRegistrationDTO.UserName,
                Email = userRegistrationDTO.Email
            };

            _userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                        .ReturnsAsync(IdentityResult.Success);
            _userManager.Setup(x => x.FindByEmailAsync(user.Email))
                        .ReturnsAsync(user);
            _userManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<User>()))
                        .ReturnsAsync("valid-token"); // Mock to return a valid token

            _emailSender.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                        .Returns(Task.CompletedTask);

            // Act
            var result = await _service.RegisterUserAsync(userRegistrationDTO);

            // Assert
            Assert.True(result.Succeeded);
        }


        [Fact]
        public async Task RegisterUserAsync_DuplicateEmail_FailsRegistration()
        {
            // Arrange
            var userRegistrationDTO = new UserRegistrationDTO
            {
                UserName = "existinguser",
                Email = "existing@example.com",
                Password = "Password123!"
            };
            var user = new User
            {
                Id = Guid.NewGuid().ToString(), // Ensure the user has an ID
                UserName = userRegistrationDTO.UserName,
                Email = userRegistrationDTO.Email
            };
            _userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                        .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Duplicate email." }));
            _userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _userManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<User>()))
                        .ReturnsAsync("dummy-token"); // Ensure a dummy token is returned

            // Act
            var result = await _service.RegisterUserAsync(userRegistrationDTO);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains(result.Errors, e => e.Description.Contains("Duplicate email"));
        }

        [Fact]
        public async Task RegisterUserAsync_PasswordWithoutDigit_FailsRegistration()
        {
            // Arrange
            var userRegistrationDTO = new UserRegistrationDTO
            {
                UserName = "user",
                Email = "user@example.com",
                Password = "Password!" // No digit
            };
            _userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                        .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Passwords must have at least one digit." }));

            // Act
            var result = await _service.RegisterUserAsync(userRegistrationDTO);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains(result.Errors, e => e.Description.Contains("digit"));
        }

        [Fact]
        public async Task RegisterUserAsync_PasswordWithoutUppercase_FailsRegistration()
        {
            // Arrange
            var userRegistrationDTO = new UserRegistrationDTO
            {
                UserName = "user",
                Email = "user@example.com",
                Password = "password1!" // No uppercase letter
            };
            _userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                        .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Passwords must have at least one uppercase ('A'-'Z')." }));

            // Act
            var result = await _service.RegisterUserAsync(userRegistrationDTO);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains(result.Errors, e => e.Description.Contains("uppercase"));
        }

        [Fact]
        public async Task RegisterUserAsync_PasswordWithoutLowercase_FailsRegistration()
        {
            // Arrange
            var userRegistrationDTO = new UserRegistrationDTO
            {
                UserName = "user",
                Email = "user@example.com",
                Password = "PASSWORD1!" // No lowercase letter
            };
            _userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                        .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Passwords must have at least one lowercase ('a'-'z')." }));

            // Act
            var result = await _service.RegisterUserAsync(userRegistrationDTO);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains(result.Errors, e => e.Description.Contains("lowercase"));
        }

        [Fact]
        public async Task RegisterUserAsync_ShortPassword_FailsRegistration()
        {
            // Arrange
            var userRegistrationDTO = new UserRegistrationDTO
            {
                UserName = "user",
                Email = "user@example.com",
                Password = "Pass1!" // Shorter than required length
            };
            _userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                        .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Passwords must be at least 6 characters." }));

            // Act
            var result = await _service.RegisterUserAsync(userRegistrationDTO);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains(result.Errors, e => e.Description.Contains("6 characters"));
        }

    }
}
