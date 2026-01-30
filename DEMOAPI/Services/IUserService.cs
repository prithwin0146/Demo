using EmployeeApi.DTOs;

namespace EmployeeApi.Services
{
    public interface IUserService
    {
        Task<string?> RegisterAsync(RegisterUserDto registerDto);
        LoginResponseDto? Login(LoginUserDto loginDto);
    }
}
