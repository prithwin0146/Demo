using EmployeeApi.Models;
using EmployeeApi.DTOs;

namespace EmployeeApi.Repositories;

public interface IDepartmentRepository
{
    Task<List<Department>> GetAllAsync();
    Task<Department?> GetByIdAsync(int id);
    Task<int> CreateAsync(Department department);
    Task<bool> UpdateAsync(int id, Department department);
    Task<bool> DeleteAsync(int id);
    Task<int> GetEmployeeCountAsync(int departmentId);
    Task<(List<DepartmentPagedResult> Data, int TotalCount)> GetDepartmentsPagedAsync(PaginationRequest request);
}
