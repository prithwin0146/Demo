using EmployeeApi.DTOs;

namespace EmployeeApi.Services;

public interface IEmployeeService
{
    Task<List<EmployeeDto>> GetAllAsync();
    Task<EmployeeDto> GetByIdAsync(int id);
    Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto);
    Task<EmployeeDto> UpdateAsync(int id, CreateEmployeeDto dto);
    Task<EmployeeDto> DeleteAsync(int id);
}
