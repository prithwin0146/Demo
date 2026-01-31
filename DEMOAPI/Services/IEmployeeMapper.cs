using EmployeeApi.Models;
using EmployeeApi.DTOs;

namespace EmployeeApi.Services;

public interface IEmployeeMapper
{
    EmployeeDto ToDto(Employee employee);
    List<EmployeeDto> ToDto(List<Employee> employees);
}

public class EmployeeMapper : IEmployeeMapper
{
    public EmployeeDto ToDto(Employee emp)
    {
        return new EmployeeDto
        {
            Id = emp.Id,
            Name = emp.Name,
            Email = emp.Email,
            JobRole = emp.JobRole,
            SystemRole = emp.Role ?? "Employee"
        };
    }

    public List<EmployeeDto> ToDto(List<Employee> employees)
    {
        return employees.Select(ToDto).ToList();
    }
}
