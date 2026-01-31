using EmployeeApi.DTOs;
using EmployeeApi.Models;
using EmployeeApi.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EmployeeApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;

        public UserService(IUserRepository userRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            _config = config;
        }

        //Register
        public async Task<string?> RegisterAsync(RegisterUserDto registerDto)
        {
            if (string.IsNullOrWhiteSpace(registerDto.Email) ||
                string.IsNullOrWhiteSpace(registerDto.Password))
                return null;

            if (_userRepository.Any(x => x.Email == registerDto.Email))
                return null;

            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                Password = registerDto.Password
            };

            _userRepository.Add(user);
            await _userRepository.SaveChangesAsync();

            return "Registered!";
        }

        //Login
        public LoginResponseDto? Login(LoginUserDto loginDto)
        {
          
            var user = _userRepository.Find(x => x.Email == loginDto.Email && x.Password == loginDto.Password);

            if (user == null)
                return null;

            var token = GenerateJwtToken(user);
            return new LoginResponseDto 
            { 
                Token = token,
                Role = user.Role ?? "Employee"  
            };
        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username ?? ""),
                new Claim(ClaimTypes.Role, user.Role ?? "Employee")
            };

            var creds = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
