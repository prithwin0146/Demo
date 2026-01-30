using EmployeeApi.Models;
using EmployeeApi.DTOs;

namespace EmployeeApi.Repositories;

public interface IEmployeeRepository
{
    Task<List<Employee>> GetAllAsync();
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee> AddAsync(Employee emp);
    Task<Employee> AddWithPasswordAsync(Employee emp, string password);
    Task<Employee> UpdateAsync(Employee emp);
    Task<Employee> DeleteAsync(Employee emp);
    Task<PagedResponse<EmployeeDto>> GetEmployeesPagedAsync(PaginationRequest request);
}
