using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IAuthService _authService;

        public AuthController(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IAuthService authService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var result = await _signInManager.PasswordSignInAsync(loginDTO.Username, loginDTO.Password, false, false);
            if (result.Succeeded)
            {
                // Генеруємо токен або використовуємо існуючий механізм для генерації токенів
                var token = await _authService.LoginAsync(loginDTO);
                return Ok(new { token });
            }
            return BadRequest("Invalid username or password");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            var user = new User
            {
                Username = registerDTO.Username,
                Email = registerDTO.Email
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            if (result.Succeeded)
            {
                // Генеруємо токен або використовуємо існуючий механізм для генерації токенів
                var token = await _authService.RegisterAsync(registerDTO);
                return Ok(new { token });
            }
            return BadRequest(result.Errors);
        }
    }
}
