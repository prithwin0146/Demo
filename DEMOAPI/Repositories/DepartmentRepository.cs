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

    public List<Department> GetAll()
    {
        return _context.Departments
            .OrderBy(d => d.DepartmentName)
            .ToList();
    }

    public Department? GetById(int id)
    {
        return _context.Departments
            .FirstOrDefault(d => d.DepartmentId == id);
    }

    public int Create(Department department)
    {
        _context.Departments.Add(department);
        _context.SaveChanges();
        return department.DepartmentId;
    }

    public bool Update(int id, Department department)
    {
        var existingDepartment = _context.Departments.Find(id);
        if (existingDepartment == null) return false;

        existingDepartment.DepartmentName = department.DepartmentName;
        existingDepartment.Description = department.Description;
        existingDepartment.ManagerId = department.ManagerId;

        _context.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var department = _context.Departments.Find(id);
        if (department == null) return false;

        _context.Departments.Remove(department);
        _context.SaveChanges();
        return true;
    }

    public int GetEmployeeCount(int departmentId)
    {
        return _context.Employees
            .Count(e => e.DepartmentId == departmentId);
    }

    public async Task<(List<DepartmentPagedResult> Data, int TotalCount)> GetDepartmentsPagedAsync(PaginationRequest request)
    {
        var results = await _context.LoadStoredProc<DepartmentPagedResult>(
            "sp_GetDepartmentsPaged",
            new SqlParameter("@PageNumber", request.PageNumber),
            new SqlParameter("@PageSize", request.PageSize),
            new SqlParameter("@SortBy", request.SortBy ?? "DepartmentName"),
            new SqlParameter("@SortOrder", request.SortOrder == "DESC" ? "DESC" : "ASC"),
            new SqlParameter("@SearchTerm", (object?)request.SearchTerm ?? DBNull.Value));

        var totalRecords = results.FirstOrDefault()?.TotalCount ?? 0;

        return (results.ToList(), totalRecords);
    }
}
