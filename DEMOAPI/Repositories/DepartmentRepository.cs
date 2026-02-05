using EmployeeApi.Models;
using EmployeeApi.DTOs;
using EmployeeApi.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace EmployeeApi.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly TaskDbContext _context;

    public DepartmentRepository(TaskDbContext context)
    {
        _context = context;
    }

    public async Task<List<Department>> GetAllAsync()
    {
        return await _context.Departments
            .OrderBy(d => d.DepartmentName)
            .ToListAsync();
    }

    public async Task<Department?> GetByIdAsync(int id)
    {
        return await _context.Departments
            .FirstOrDefaultAsync(d => d.DepartmentId == id);
    }

    public async Task<int> CreateAsync(Department department)
    {
        _context.Departments.Add(department);
        await _context.SaveChangesAsync();
        return department.DepartmentId;
    }

    public async Task<bool> UpdateAsync(int id, Department department)
    {
        var existingDepartment = await _context.Departments.FindAsync(id);
        if (existingDepartment == null) return false;

        existingDepartment.DepartmentName = department.DepartmentName;
        existingDepartment.Description = department.Description;
        existingDepartment.ManagerId = department.ManagerId;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null) return false;

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetEmployeeCountAsync(int departmentId)
    {
        return await _context.Employees
            .CountAsync(e => e.DepartmentId == departmentId);
    }

    public async Task<(List<DepartmentPagedResult> Data, int TotalCount)> GetDepartmentsPagedAsync(PaginationRequest request)
    {
        var results = await _context.LoadStoredProc<DepartmentPagedResult>(
            "sp_GetDepartmentsPaged",
            new SqlParameter("@PageNumber", request.PageNumber),
            new SqlParameter("@PageSize", request.PageSize),
            new SqlParameter("@SortBy", request.SortBy ?? "DepartmentName"),
            new SqlParameter("@SortOrder", request.Ascending ? "ASC" : "DESC"),
            new SqlParameter("@SearchTerm", (object?)request.SearchTerm ?? DBNull.Value));

        var totalRecords = results.FirstOrDefault()?.TotalCount ?? 0;

        return (results.ToList(), totalRecords);
    }
}
