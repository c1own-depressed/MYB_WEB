using System.Net;
using System.Net.Mail;
using Application.DTOs.AccountDTOs;
using Application.Interfaces;
using Azure.Core;
using Domain.Entities;
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

        public async Task SendEmailConfirmationAsync(User user)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //var callbackUrl = $"https://yourdomain.com/account/confirmemail?userId={user.Id}&code={Uri.EscapeDataString(code)}";
            var callbackUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/account/confirmemail?userId={user.Id}&code={Uri.EscapeDataString(code)}";
            //await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
            //    $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.");
            await SendEmailAsync(user.Email, "Confirm your email",
                $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.");
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
                return false;
            }
        }
    }
}
