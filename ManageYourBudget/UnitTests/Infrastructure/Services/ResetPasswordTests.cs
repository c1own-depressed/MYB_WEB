//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.UI.Services;
//using Xunit;
//using Moq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Identity;
//using Application.DTOs.AccountDTOs;
//using Application.Interfaces;
//using Domain.Entities;
//using Microsoft.AspNetCore.Http;
//using Persistence.AuthService;

//namespace UnitTests.Infrastructure.Services
//{
//    public class ResetPasswordTests
//    {
//        private readonly Mock<UserManager<User>> _userManager;
//        private readonly Mock<IEmailSender> _emailSender;
//        private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
//        private readonly AuthService _service;

//        public ResetPasswordTests()
//        {
//            _userManager = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null, null, null);
//            _emailSender = new Mock<IEmailSender>();
//            _httpContextAccessor = new Mock<IHttpContextAccessor>();

//            _service = new AuthService(_userManager.Object, _emailSender.Object, _httpContextAccessor.Object);
//        }

//        [Fact]
//        public async Task ResetPasswordAsync_ValidData_SendsEmail()
//        {
//            // Arrange
//            var resetPasswordDTO = new ResetPasswordDTO
//            {
//                Email = "user@example.com",
//                Token = "some_token",
//                Password = "newpassword",
//                ConfirmPassword = "newpassword"
//            };

//            _userManager.Setup(x => x.FindByEmailAsync(resetPasswordDTO.Email)).ReturnsAsync(new User());

//            // Act
//            var result = await _service.ResetPasswordAsync(resetPasswordDTO);

//            // Assert
//            Assert.True(result.Succeeded);
//            _emailSender.Verify(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
//        }

//        [Fact]
//        public async Task ResetPasswordAsync_ValidData_SendsCorrectEmail()
//        {
//            // Arrange
//            var user = new User { Email = "user@example.com" };
//            var resetPasswordDTO = new ResetPasswordDTO
//            {
//                Email = user.Email,
//                Token = "some_token",
//                Password = "newpassword",
//                ConfirmPassword = "newpassword"
//            };

//            _userManager.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(user);

//            // Act
//            var result = await _service.ResetPasswordAsync(resetPasswordDTO);

//            // Assert
//            Assert.True(result.Succeeded);
//            _emailSender.Verify(x => x.SendEmailAsync(user.Email, "Reset your password", It.IsAny<string>()), Times.Once);
//        }
//    }
//}
