using System;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Core.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }

        public async Task<string> LoginAsync(LoginDTO loginDTO)
        {
            var result = await _signInManager.PasswordSignInAsync(loginDTO.Username, loginDTO.Password, false, false);

            if (result.Succeeded)
            {
                // Вхід успішний, генеруємо токен або повертаємо підтвердження успішної автентифікації
                return "your_generated_token_here";
            }
            else
            {
                // Вхід неуспішний, повертаємо null
                return null;
            }
        }

        public async Task<string> RegisterAsync(RegisterDTO registerDTO)
        {
            var existingUser = await _userManager.FindByNameAsync(registerDTO.Username);
            if (existingUser != null)
            {
                throw new Exception("Username is already taken");
            }

            existingUser = await _userManager.FindByEmailAsync(registerDTO.Email);
            if (existingUser != null)
            {
                throw new Exception("Email is already registered");
            }

            var newUser = new User
            {
                Username = registerDTO.Username,
                Email = registerDTO.Email
            };

            var result = await _userManager.CreateAsync(newUser, registerDTO.Password);

            if (result.Succeeded)
            {
                // Реєстрація успішна, генеруємо токен або повертаємо підтвердження успішної реєстрації
                return "your_generated_token_here";
            }
            else
            {
                // Помилка при реєстрації, повертаємо null або генеруємо помилку
                throw new Exception("Registration failed");
            }
        }
    }
}
