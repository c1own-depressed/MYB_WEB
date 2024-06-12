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
using Microsoft.Extensions.Logging;

namespace Persistence.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthService> _logger;

        public AuthService(UserManager<User> userManager, IEmailSender emailSender, IHttpContextAccessor httpContextAccessor, ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<IdentityResult> RegisterUserAsync(UserRegistrationDTO userRegistrationDTO)
        {
            _logger.LogInformation($"Registering user: {userRegistrationDTO.Email}");

            try
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
                    _logger.LogInformation($"User registered successfully: {userRegistrationDTO.Email}");
                    // await SendEmailConfirmationAsync(user);

                    // Automatically confirm email
                    var confirmEmailResult = await ConfirmEmailAsync(user.Id, await _userManager.GenerateEmailConfirmationTokenAsync(user));
                    if (confirmEmailResult.Succeeded)
                    {
                        _logger.LogInformation($"Email confirmed automatically for user: {userRegistrationDTO.Email}");
                    }
                    else
                    {
                        _logger.LogWarning($"Automatic email confirmation failed for user: {userRegistrationDTO.Email}");
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error registering user: {userRegistrationDTO.Email}");
                throw; // Re-throw the exception for handling in upper layers
            }
        }

        public async Task<User?> AuthenticateUserAsync(UserLoginDTO userLoginDTO)
        {
            _logger.LogInformation($"Authenticating user: {userLoginDTO.Email}");

            try
            {
                var user = await _userManager.FindByEmailAsync(userLoginDTO.Email);

                if (user == null)
                {
                    _logger.LogInformation($"User not found: {userLoginDTO.Email}");
                    return null; // User not found
                }

                var result = await _userManager.CheckPasswordAsync(user, userLoginDTO.Password);

                if (!result)
                {
                    _logger.LogInformation($"Invalid password for user: {userLoginDTO.Email}");
                    return null; // Invalid password
                }

                _logger.LogInformation($"User authenticated successfully: {userLoginDTO.Email}");

                // Create claims for the authenticated user
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
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
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error authenticating user: {userLoginDTO.Email}");
                throw; // Re-throw the exception for handling in upper layers
            }
        }

        public async Task SendEmailConfirmationAsync(User user)
        {
            _logger.LogInformation($"Sending email confirmation to user: {user.Email}");

            try
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var callbackUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/account/confirmemail?userId={user.Id}&code={Uri.EscapeDataString(code)}";

                string htmlMessage = $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.";
                await SendEmailAsync(user.Email, "Confirm your email", htmlMessage);

                _logger.LogInformation($"Email confirmation sent successfully to user: {user.Email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending email confirmation to user: {user.Email}");
                throw; // Re-throw the exception for handling in upper layers
            }
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
                _logger.LogError(ex, $"Error sending email to: {email}");
                return false;
            }
        }

        public async Task<IdentityResult> ForgotPasswordAsync(string email)
        {
            _logger.LogInformation($"Forgot password request initiated for email: {email}");

            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    _logger.LogWarning($"User not found or not confirmed for email: {email}");
                    return IdentityResult.Failed(new IdentityError { Description = "User not found or not confirmed." });
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/resetpassword?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";

                // Send email
                await SendEmailAsync(email, "Reset Password", $"Please reset your password by clicking <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>here</a>.");

                _logger.LogInformation($"Reset password email sent successfully to: {email}");
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing forgot password request for email: {email}");
                throw; // Re-throw the exception for handling in upper layers
            }
        }

        public async Task LogoutAsync()
        {
            _logger.LogInformation("User logout initiated");

            try
            {
                // Sign out the current user
                await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                _logger.LogInformation("User logged out successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user logout");
                throw; // Re-throw the exception for handling in upper layers
            }
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO)
        {
            _logger.LogInformation($"Resetting password for user: {resetPasswordDTO.Email}");

            try
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

                _logger.LogInformation($"Password reset successful for user: {resetPasswordDTO.Email}");

                // Optionally: Sign the user in directly after password reset
                var signInResult = await SignInUser(user);

                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while resetting password for user: {resetPasswordDTO.Email}");
                throw; // Re-throw the exception for handling in upper layers
            }
        }

        public async Task<IdentityResult> ConfirmEmailAsync(string userId, string token)
        {
            _logger.LogInformation($"Confirming email for user ID: {userId}");

            try
            {
                if (userId == null || token == null)
                    return IdentityResult.Failed(new IdentityError { Description = "Invalid user ID or token." });

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return IdentityResult.Failed(new IdentityError { Description = "User not found." });

                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"Email confirmed successfully for user ID: {userId}");
                }
                else
                {
                    _logger.LogWarning($"Email confirmation failed for user ID: {userId}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while confirming email for user ID: {userId}");
                throw; // Re-throw the exception for handling in upper layers
            }
        }

        private async Task<IdentityResult> SignInUser(User user)
        {
            _logger.LogInformation($"Signing in user: {user.Email}");

            try
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, user.Id) };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                var properties = new AuthenticationProperties { IsPersistent = true };

                await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);

                _logger.LogInformation($"User signed in successfully: {user.Email}");
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while signing in user: {user.Email}");
                throw; // Re-throw the exception for handling in upper layers
            }
        }
    }
}
