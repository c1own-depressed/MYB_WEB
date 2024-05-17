using Application.DTOs.AccountDTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterUserAsync(UserRegistrationDTO userRegistrationDTO);

        Task SendEmailConfirmationAsync(User user);

        Task<User> AuthenticateUserAsync(UserLoginDTO userLoginDTO);

        Task<IdentityResult> ForgotPasswordAsync(string email);

        Task LogoutAsync();

        Task<IdentityResult> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO);

        Task<IdentityResult> ConfirmEmailAsync(string userId, string token);
    }
}
