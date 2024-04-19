using Application.DTOs.AccountDTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterUserAsync(UserRegistrationDTO userRegistrationDTO);

        Task SendEmailConfirmationAsync(User user);

        Task<User> AuthenticateUserAsync(UserLoginDTO userLoginDTO);
    }
}
