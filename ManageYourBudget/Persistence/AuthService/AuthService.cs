using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using Application.DTOs.AccountDTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Persistence.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(UserManager<User> userManager, IEmailSender emailSender, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IdentityResult> RegisterUserAsync(UserRegistrationDTO userRegistrationDTO)
        {
            User user = new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = userRegistrationDTO.UserName,
                Email = userRegistrationDTO.Email,
                IsLightTheme = true,
                Currency = "uah",
                Language = "uk",
            };

            var result = await _userManager.CreateAsync(user, userRegistrationDTO.Password);
            if (result.Succeeded)
            {
                await SendEmailConfirmationAsync(user);
            }

            return result;
        }

        public async Task<User?> AuthenticateUserAsync(UserLoginDTO userLoginDTO)
        {
            var user = await _userManager.FindByEmailAsync(userLoginDTO.Email);

            if (user == null)
            {
                return null; // User not found
            }

            var result = await _userManager.CheckPasswordAsync(user, userLoginDTO.Password);

            if (!result)
            {
                return null; // Invalid password
            }

            // Create claims for the authenticated user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };

            // Create authentication properties
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Persist the cookie across browser sessions
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30), // Cookie expiration time
            };

            // Create the principal
            var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Sign in the user
            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(userIdentity),
                authProperties);

            // Return the authenticated user
            return user;
        }

        public async Task SendEmailConfirmationAsync(User user)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var callbackUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/account/confirmemail?userId={user.Id}&code={Uri.EscapeDataString(code)}";

            string htmlMessage = $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.";
            _ = await SendEmailAsync(user.Email, "Confirm your email", htmlMessage);
        }

        private async Task<bool> SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                using (var mailMessage = new MailMessage())
                using (var smtpClient = new SmtpClient("localhost", 1025)) // Use MailHog's default port
                {
                    mailMessage.From = new MailAddress("manageyourbudget@myb.com");
                    mailMessage.To.Add(new MailAddress(email));
                    mailMessage.Subject = subject;
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Body = htmlMessage;

                    await smtpClient.SendMailAsync(mailMessage);
                    return true;
                }
            }
            catch (Exception ex)
            {
                // _logger.Error(ex);
                return false;
            }
        }
    }
}
