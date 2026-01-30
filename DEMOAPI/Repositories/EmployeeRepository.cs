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

    public async Task<List<Employee>> GetAllAsync()
    {
        return await _context.LoadStoredProc<Employee>("sp_GetAllEmployees");
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        return await _context.LoadStoredProcSingle<Employee>(
            "sp_GetEmployeeById",
            new SqlParameter("@Id", id));
    }

    public async Task<Employee> AddAsync(Employee emp)
    {
        var outputParam = new SqlParameter("@NewEmployeeId", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };

        var (results, newId) = await _context.LoadStoredProcWithOutput<Employee>(
            "sp_AddEmployee",
            outputParam,
            new SqlParameter("@Name", emp.Name),
            new SqlParameter("@Email", emp.Email),
            new SqlParameter("@JobRole", emp.JobRole ?? (object)DBNull.Value),
            new SqlParameter("@Role", emp.Role ?? "Employee"));

        return results.FirstOrDefault() ?? throw new Exception("Failed to add employee");
    }

    public async Task<Employee> AddWithPasswordAsync(Employee emp, string password)
    {
        var outputParam = new SqlParameter("@NewEmployeeId", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output
        };

        var (results, newId) = await _context.LoadStoredProcWithOutput<Employee>(
            "sp_CreateEmployee",
            outputParam,
            new SqlParameter("@Name", emp.Name),
            new SqlParameter("@Email", emp.Email),
            new SqlParameter("@JobRole", emp.JobRole ?? (object)DBNull.Value),
            new SqlParameter("@SystemRole", emp.Role ?? "Employee"),
            new SqlParameter("@Password", password));

        return results.FirstOrDefault() ?? throw new Exception("Failed to create employee");
    }

    public async Task<Employee> UpdateAsync(Employee emp)
    {
        var results = await _context.LoadStoredProc<Employee>(
            "sp_UpdateEmployee",
            new SqlParameter("@Id", emp.Id),
            new SqlParameter("@Name", emp.Name),
            new SqlParameter("@Email", emp.Email),
            new SqlParameter("@JobRole", emp.JobRole ?? (object)DBNull.Value),
            new SqlParameter("@SystemRole", emp.Role ?? "Employee"));

        return results.FirstOrDefault() ?? throw new Exception("Failed to update employee");
    }

    public async Task<Employee> DeleteAsync(Employee emp)
    {
        var results = await _context.LoadStoredProc<Employee>(
            "sp_DeleteEmployee",
            new SqlParameter("@Id", emp.Id));

        return results.FirstOrDefault() ?? throw new Exception("Employee not found");
    }

    public async Task<PagedResponse<EmployeeDto>> GetEmployeesPagedAsync(PaginationRequest request)
    {
        var results = await _context.LoadStoredProc<EmployeePagedResult>(
            "sp_GetEmployeesPaged",
            new SqlParameter("@PageNumber", request.PageNumber),
            new SqlParameter("@PageSize", request.PageSize),
            new SqlParameter("@SortBy", request.SortBy ?? "Id"),
            new SqlParameter("@SortOrder", request.Ascending ? "ASC" : "DESC"),
            new SqlParameter("@SearchTerm", (object?)request.SearchTerm ?? DBNull.Value));

        var totalRecords = results.FirstOrDefault()?.TotalCount ?? 0;
        var totalPages = (int)Math.Ceiling(totalRecords / (double)request.PageSize);

        var employeeDtos = results.Select(r => new EmployeeDto
        {
            Id = r.Id,
            Name = r.Name,
            Email = r.Email,
            JobRole = r.JobRole,
            SystemRole = r.Role ?? "Employee"
        }).ToList();

        return new PagedResponse<EmployeeDto>
        {
            Data = employeeDtos,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRecords = totalRecords,
            TotalPages = totalPages
        };
    }
}
