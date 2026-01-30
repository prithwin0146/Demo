namespace EmployeeApi.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;  // Admin | HR | Employee
    }

    public class RegisterUserDto
    {
        public string? Username { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = "Employee";  // Default to Employee
    }

    public class LoginUserDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class LoginResponseDto
    {
        public string Token { get; set; } = null!;
        public string Role { get; set; } = null!;  // Include role in login response
    }
}
