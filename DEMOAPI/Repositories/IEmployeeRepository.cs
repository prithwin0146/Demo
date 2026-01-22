using EmployeeApi.Models;

namespace EmployeeApi.Repositories;

public interface IEmployeeRepository
{
    Task<List<Employee>> GetAllAsync();
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee> AddAsync(Employee emp);
    Task<Employee> UpdateAsync(Employee emp);
    Task<Employee> DeleteAsync(Employee emp);
}
