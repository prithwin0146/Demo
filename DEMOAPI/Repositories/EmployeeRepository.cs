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
        return await _context.LoadStoredProc<Employee>("GetAllEmployees");
    }
    // Get by ID 
    public async Task<Employee?> GetByIdAsync(int id)
    {
        return await _context.LoadStoredProcSingle<Employee>(
            "GetEmployeeById",
            new SqlParameter("@Id", id));
    }
    // Create (without password)
    public async Task<Employee> AddAsync(Employee emp)
    {
        var results = await _context.LoadStoredProc<Employee>(
            "CreateEmployeeWithoutPassword",
            new SqlParameter("@Name", emp.Name),
            new SqlParameter("@Email", emp.Email),
            new SqlParameter("@JobRole", emp.JobRole ?? (object)DBNull.Value),
            new SqlParameter("@SystemRole", emp.Role ?? "Employee"));

        return results.FirstOrDefault() ?? throw new Exception("Failed to create employee");
    }
    // Create (with password)
    public async Task<Employee> AddWithPasswordAsync(Employee emp, string password)
    {
        var outputParam = new SqlParameter("@NewEmployeeId", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };

        var (results, newId) = await _context.LoadStoredProcWithOutput<Employee>(
            "CreateEmployee",
            outputParam,

            new SqlParameter("@Name", emp.Name),
            new SqlParameter("@Email", emp.Email),
            new SqlParameter("@JobRole", emp.JobRole ?? (object)DBNull.Value),
            new SqlParameter("@SystemRole", emp.Role ?? "Employee"),
            new SqlParameter("@Password", password));

        return results.FirstOrDefault() ?? throw new Exception("Failed to create employee");
    }
    // Update 
    public async Task<Employee> UpdateAsync(Employee emp)
    {
        var results = await _context.LoadStoredProc<Employee>(
            "UpdateEmployee",
            new SqlParameter("@Id", emp.Id),
            new SqlParameter("@Name", emp.Name),
            new SqlParameter("@Email", emp.Email),
            new SqlParameter("@JobRole", emp.JobRole ?? (object)DBNull.Value),
            new SqlParameter("@SystemRole", emp.Role ?? "Employee"));

        return results.FirstOrDefault() ?? throw new Exception("Failed to update employee");
    }
    // Delete 
    public async Task<Employee> DeleteAsync(Employee emp)
    {
        var results = await _context.LoadStoredProc<Employee>(
            "DeleteEmployee",
            new SqlParameter("@Id", emp.Id));

        return results.FirstOrDefault() ?? throw new Exception("Employee not found");
    }
    // Pagination 
    public async Task<(List<Employee> Data, int TotalCount)> GetEmployeesPagedAsync(PaginationRequest request)
    {
        var results = await _context.LoadStoredProc<EmployeePagedResult>(
            "sp_GetEmployeesPaged",
            new SqlParameter("@PageNumber", request.PageNumber),
            new SqlParameter("@PageSize", request.PageSize),
            new SqlParameter("@SortBy", request.SortBy ?? "Id"),
            new SqlParameter("@SortOrder", request.Ascending ? "ASC" : "DESC"),
            new SqlParameter("@SearchTerm", (object?)request.SearchTerm ?? DBNull.Value));

        var totalRecords = results.FirstOrDefault()?.TotalCount ?? 0;

        var employees = results.Select(r => new Employee
        {
            Id = r.Id,
            Name = r.Name,
            Email = r.Email,
            JobRole = r.JobRole,
            Role = r.Role
        }).ToList();

        return (employees, totalRecords);
    }
}
