using EmployeeApi.Models;
using EmployeeApi.DTOs;

namespace EmployeeApi.Repositories;

public interface IEmployeeRepository
{
    List<Employee> GetAll();
    Employee? GetById(int id);
    Employee Add(Employee emp);
    Employee AddWithPassword(Employee emp, string password);
    Employee Update(Employee emp);
    Employee Delete(Employee emp);
    bool ExistsByEmail(string email);
    Task<(List<Employee> Data, int TotalCount)> GetEmployeesPagedAsync(PaginationRequest request, int? departmentId = null, string? jobRole = null, string? systemRole = null, int? projectId = null);
}
