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
    public List<Employee> GetAll()
    {
        return _context.Employees
            .OrderBy(e => e.Id)
            .ToList();
    }

    // Get by ID 
    public Employee? GetById(int id)
    {
        return _context.Employees
            .FirstOrDefault(e => e.Id == id);
    }

    // Create (without password) 
    public Employee Add(Employee emp)
    {
        emp.Role ??= "Employee";
        _context.Employees.Add(emp);
        _context.SaveChanges();
        return emp;
    }

    // Create (with password) 
    public Employee AddWithPassword(Employee emp, string password)
    {
        emp.Role ??= "Employee";

        using var transaction = _context.Database.BeginTransaction();
        try
        {
            // Create employee
            _context.Employees.Add(emp);
            _context.SaveChanges();

            // Create corresponding user with password
            var user = new User
            {
                Username = emp.Name,
                Email = emp.Email,
                Password = password,
                Role = emp.Role
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            transaction.Commit();
            return emp;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    // Check if employee exists by email
    public bool ExistsByEmail(string email)
    {
        return _context.Employees.Any(e => e.Email == email);
    }

    // Update 
    public Employee Update(Employee emp)
    {
        emp.Role ??= "Employee";
        _context.Employees.Update(emp);
        _context.SaveChanges();
        return emp;
    }

    // Delete
    public Employee Delete(Employee emp)
    {
        _context.Employees.Remove(emp);
        _context.SaveChanges();
        return emp;
    }
    // Pagination 
    public async Task<(List<Employee> Data, int TotalCount)> GetEmployeesPagedAsync(PaginationRequest request, int? departmentId = null, string? jobRole = null, string? systemRole = null)
    {
        var results = await _context.LoadStoredProc<EmployeePagedResult>(
            "sp_GetEmployeesPaged",
            new SqlParameter("@PageNumber", request.PageNumber),
            new SqlParameter("@PageSize", request.PageSize),
            new SqlParameter("@SortBy", request.SortBy ?? "Id"),
            new SqlParameter("@SortOrder", request.SortOrder == "DESC" ? "DESC" : "ASC"),
            new SqlParameter("@SearchTerm", (object?)request.SearchTerm ?? DBNull.Value),
            new SqlParameter("@DepartmentId", (object?)departmentId ?? DBNull.Value),
            new SqlParameter("@JobRole", (object?)jobRole ?? DBNull.Value),
            new SqlParameter("@SystemRole", (object?)systemRole ?? DBNull.Value));

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
