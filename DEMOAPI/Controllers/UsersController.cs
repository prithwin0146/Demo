using Microsoft.AspNetCore.Mvc;
using EmployeeApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly TaskDbContext _context;
        private readonly IConfiguration _config;

        public UsersController(TaskDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // REGISTER
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (
                string.IsNullOrWhiteSpace(user.Email) ||
                string.IsNullOrWhiteSpace(user.Password))
                return BadRequest("All fields are required.");

            if (await _context.Users.AnyAsync(x => x.Email == user.Email))
                return BadRequest("Email already exists.");

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Registered!");
        }

        // GET ALL USERS
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new { u.Id, u.Username, u.Email, u.Role })
                .ToListAsync();
            return Ok(users);
        }

        // LOGIN
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User loginUser)
        {
            
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == loginUser.Email && x.Password == loginUser.Password);

            if (existingUser == null)
                return Unauthorized(new { message = "Invalid credentials" });

           
            var token = GenerateJwtToken(existingUser);
            return Ok(new { 
                token, 
                role = existingUser.Role ?? "Employee",
                username = existingUser.Username,
                email = existingUser.Email
            });
        }
        // JWT
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
