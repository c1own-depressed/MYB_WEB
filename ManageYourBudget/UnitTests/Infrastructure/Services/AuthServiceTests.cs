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
        public async Task AuthenticateUserAsync_InValidCredentials_NoUpperCaseLetterPassword_ReturnsUser()
        {
            // Arrange
            var user = new User { Email = "user@example.com" };
            _userManager.Setup(x => x.FindByEmailAsync("user@example.com")).ReturnsAsync(user);
            _userManager.Setup(x => x.CheckPasswordAsync(user, "correctpassword123")).ReturnsAsync(false);
            _authService.Setup(x =>
                x.SignInAsync(
                    It.IsAny<HttpContext>(),
                    It.IsAny<string>(),
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<AuthenticationProperties>()
                )
            ).Returns(Task.CompletedTask).Verifiable("SignIn was not called.");

            // Act
            var result = await _service.AuthenticateUserAsync(new UserLoginDTO { Email = "user@example.com", Password = "correctpassword123" });

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
        public async Task AuthenticateUserAsync_InValidCredentials_NoLowerCaseLetterPassword_ReturnsUser()
        {
            // Arrange
            var user = new User { Email = "user@example.com" };
            _userManager.Setup(x => x.FindByEmailAsync("user@example.com")).ReturnsAsync(user);
            _userManager.Setup(x => x.CheckPasswordAsync(user, "CORRECTPASSWORD123")).ReturnsAsync(false);
            _authService.Setup(x =>
                x.SignInAsync(
                    It.IsAny<HttpContext>(),
                    It.IsAny<string>(),
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<AuthenticationProperties>()
                )
            ).Returns(Task.CompletedTask).Verifiable("SignIn was not called.");

            // Act
            var result = await _service.AuthenticateUserAsync(new UserLoginDTO { Email = "user@example.com", Password = "CORRECTPASSWORD123" });

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
        public async Task AuthenticateUserAsync_InValidCredentials_NoNumbersPassword_ReturnsUser()
        {
            // Arrange
            var user = new User { Email = "user@example.com" };
            _userManager.Setup(x => x.FindByEmailAsync("user@example.com")).ReturnsAsync(user);
            _userManager.Setup(x => x.CheckPasswordAsync(user, "correctpassword")).ReturnsAsync(false);
            _authService.Setup(x =>
                x.SignInAsync(
                    It.IsAny<HttpContext>(),
                    It.IsAny<string>(),
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<AuthenticationProperties>()
                )
            ).Returns(Task.CompletedTask).Verifiable("SignIn was not called.");

            // Act
            var result = await _service.AuthenticateUserAsync(new UserLoginDTO { Email = "user@example.com", Password = "correctpassword" });

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
        public async Task AuthenticateUserAsync_InValidCredentials_ShortPassword_ReturnsUser()
        {
            // Arrange
            var user = new User { Email = "user@example.com" };
            _userManager.Setup(x => x.FindByEmailAsync("user@example.com")).ReturnsAsync(user);
            _userManager.Setup(x => x.CheckPasswordAsync(user, "corre")).ReturnsAsync(false);
            _authService.Setup(x =>
                x.SignInAsync(
                    It.IsAny<HttpContext>(),
                    It.IsAny<string>(),
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<AuthenticationProperties>()
                )
            ).Returns(Task.CompletedTask).Verifiable("SignIn was not called.");

            // Act
            var result = await _service.AuthenticateUserAsync(new UserLoginDTO { Email = "user@example.com", Password = "corre" });

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
    }
}
