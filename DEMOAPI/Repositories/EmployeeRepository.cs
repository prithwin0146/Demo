using EmployeeApi.Models;
using EmployeeApi.DTOs;
using EmployeeApi.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeApi.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly TaskDbContext _context;

    public EmployeeRepository(TaskDbContext context)
    {
        _context = context;
    }

    // Get all 
    public async Task<List<Employee>> GetAllAsync()
    {
        return await _context.Employees
            .OrderBy(e => e.Id)
            .ToListAsync();
    }

    // Get by ID 
    public async Task<Employee?> GetByIdAsync(int id)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    // Create (without password) 
    public async Task<Employee> AddAsync(Employee emp)
    {
        emp.Role ??= "Employee";
        _context.Employees.Add(emp);
        await _context.SaveChangesAsync();
        return emp;
    }

    // Create (with password) 
    public async Task<Employee> AddWithPasswordAsync(Employee emp, string password)
    {
        emp.Role ??= "Employee";
        
        // Create employee
        _context.Employees.Add(emp);
        await _context.SaveChangesAsync();

        // Create corresponding user with password
        var user = new User
        {
            Username = emp.Name,
            Email = emp.Email,
            Password = password,
            Role = emp.Role
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return emp;
    }

    // Update 
    public async Task<Employee> UpdateAsync(Employee emp)
    {
        emp.Role ??= "Employee";
        _context.Employees.Update(emp);
        await _context.SaveChangesAsync();
        return emp;
    }

    // Delete
    public async Task<Employee> DeleteAsync(Employee emp)
    {
        _context.Employees.Remove(emp);
        await _context.SaveChangesAsync();
        return emp;
    }
    // Pagination 
    public async Task<(List<Employee> Data, int TotalCount)> GetEmployeesPagedAsync(PaginationRequest request, int? departmentId = null)
    {
        var results = await _context.LoadStoredProc<EmployeePagedResult>(
            "sp_GetEmployeesPaged",
            new SqlParameter("@PageNumber", request.PageNumber),
            new SqlParameter("@PageSize", request.PageSize),
            new SqlParameter("@SortBy", request.SortBy ?? "Id"),
            new SqlParameter("@SortOrder", request.Ascending ? "ASC" : "DESC"),
            new SqlParameter("@SearchTerm", (object?)request.SearchTerm ?? DBNull.Value),
            new SqlParameter("@DepartmentId", (object?)departmentId ?? DBNull.Value));

        var totalRecords = results.FirstOrDefault()?.TotalCount ?? 0;

        var employees = results.Select(r => new Employee
        {
            Id = r.Id,
            Name = r.Name,
            Email = r.Email,
            JobRole = r.JobRole,
            Role = r.Role,
            DepartmentId = r.DepartmentId
        }).ToList();

        return (employees, totalRecords);
    }
}
