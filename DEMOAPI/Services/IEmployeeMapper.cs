using EmployeeApi.Models;
using EmployeeApi.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApi.Services;

public interface IEmployeeMapper
{
    EmployeeDto ToDto(Employee employee);
    List<EmployeeDto> ToDto(List<Employee> employees);
    Task<EmployeeDto> ToDtoAsync(Employee employee, TaskDbContext context);
    Task<List<EmployeeDto>> ToDtoAsync(List<Employee> employees, TaskDbContext context);
}

public class EmployeeMapper : IEmployeeMapper
{
    private readonly IUrlEncryptionService _urlEncryption;

    public EmployeeMapper(IUrlEncryptionService urlEncryption)
    {
        _urlEncryption = urlEncryption;
    }

    public EmployeeDto ToDto(Employee emp)
    {
        return new EmployeeDto
        {
            Id = emp.Id,
            EncryptedId = _urlEncryption.Encrypt(emp.Id),
            Name = emp.Name,
            Email = emp.Email,
            JobRole = emp.JobRole,
            SystemRole = emp.Role ?? "Employee",
            DepartmentId = emp.DepartmentId,
            DepartmentName = null
        };
    }

    public List<EmployeeDto> ToDto(List<Employee> employees)
    {
        return employees.Select(ToDto).ToList();
    }

    public async Task<EmployeeDto> ToDtoAsync(Employee emp, TaskDbContext context)
    {
        string? departmentName = null;
        if (emp.DepartmentId.HasValue)
        {
            var department = await context.Departments.FindAsync(emp.DepartmentId.Value);
            departmentName = department?.DepartmentName;
        }

        return new EmployeeDto
        {
            Id = emp.Id,
            EncryptedId = _urlEncryption.Encrypt(emp.Id),
            Name = emp.Name,
            Email = emp.Email,
            JobRole = emp.JobRole,
            SystemRole = emp.Role ?? "Employee",
            DepartmentId = emp.DepartmentId,
            DepartmentName = departmentName
        };
    }

    public async Task<List<EmployeeDto>> ToDtoAsync(List<Employee> employees, TaskDbContext context)
    {
        var result = new List<EmployeeDto>();
        foreach (var emp in employees)
        {
            result.Add(await ToDtoAsync(emp, context));
        }
        return result;
    }
}
