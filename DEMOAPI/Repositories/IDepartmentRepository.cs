using EmployeeApi.Models;
using EmployeeApi.DTOs;

namespace EmployeeApi.Repositories;

public interface IDepartmentRepository
{
    List<Department> GetAll();
    Department? GetById(int id);
    int Create(Department department);
    bool Update(int id, Department department);
    bool Delete(int id);
    int GetEmployeeCount(int departmentId);
    Task<(List<DepartmentPagedResult> Data, int TotalCount)> GetDepartmentsPagedAsync(PaginationRequest request);
}
