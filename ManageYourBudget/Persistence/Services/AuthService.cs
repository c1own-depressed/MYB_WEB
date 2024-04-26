using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Policy;
using System.Text.Encodings.Web;
using Application.DTOs.AccountDTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

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

        public async Task<IdentityResult> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed for security reasons
                return IdentityResult.Failed(new IdentityError { Description = "User not found or not confirmed." });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/resetpassword?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";

            // Send email
            await SendEmailAsync(email, "Reset Password", $"Please reset your password by clicking <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>here</a>.");

            return IdentityResult.Success;
        }

        public async Task LogoutAsync()
        {
            // Sign out the current user
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO)
        {
            if (resetPasswordDTO == null)
                return IdentityResult.Failed(new IdentityError { Description = "ResetPasswordDTO cannot be null." });

            var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "No user associated with this email." });

            // Ensure the token is valid
            if (!await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", resetPasswordDTO.Token))
                return IdentityResult.Failed(new IdentityError { Description = "Invalid token for password reset." });

            // Check password confirmation
            if (resetPasswordDTO.Password != resetPasswordDTO.ConfirmPassword)
                return IdentityResult.Failed(new IdentityError { Description = "Password and confirmation password do not match." });

            // Attempt to reset the password
            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDTO.Token, resetPasswordDTO.Password);
            if (!result.Succeeded)
                return result;

            // Optionally: Sign the user in directly after password reset
            var signInResult = await SignInUser(user);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> ConfirmEmailAsync(string userId, string token)
        {
            if (userId == null || token == null)
                return IdentityResult.Failed(new IdentityError { Description = "Invalid user ID or token." });

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result;
        }


        private async Task<IdentityResult> SignInUser(User user)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, user.Id) };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var properties = new AuthenticationProperties { IsPersistent = true };

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
            return IdentityResult.Success;
        }
    }
}
