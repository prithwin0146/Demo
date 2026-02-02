using EmployeeApi.Models;
using EmployeeApi.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace EmployeeApi.Repositories;

public interface IEmployeeProjectRepository
{
    Task<List<EmployeeProjectDto>> GetByProjectIdAsync(int projectId);
    Task<int> AssignAsync(AssignEmployeeDto dto);
    Task<bool> RemoveAsync(int employeeId, int projectId);
}

public class EmployeeProjectRepository : IEmployeeProjectRepository
{
    private readonly TaskDbContext _context;

    public EmployeeProjectRepository(TaskDbContext context)
    {
        _context = context;
    }
    // GET BY PROJECT ID
    public async Task<List<EmployeeProjectDto>> GetByProjectIdAsync(int projectId)
    {
        var projectIdParam = new SqlParameter("@ProjectId", projectId);
        
        var sql = "EXEC sp_GetEmployeeProjectsByProjectId @ProjectId";
        
        var results = await _context.Database
            .SqlQueryRaw<EmployeeProjectDto>(sql, projectIdParam)
            .ToListAsync();
            
        return results;
    }
    // ASSIGN EMPLOYEE TO PROJECT
    public async Task<int> AssignAsync(AssignEmployeeDto dto)
    {
        var employeeIdParam = new SqlParameter("@EmployeeId", dto.EmployeeId);
        var projectIdParam = new SqlParameter("@ProjectId", dto.ProjectId);
        var roleParam = new SqlParameter("@Role", (object?)dto.Role ?? DBNull.Value);
        
        var sql = "EXEC sp_AssignEmployeeToProject @EmployeeId, @ProjectId, @Role";
        
        var result = await _context.Database
            .SqlQueryRaw<int>(sql, employeeIdParam, projectIdParam, roleParam)
            .ToListAsync();
            
        return result.FirstOrDefault();
    }
    // REMOVE EMPLOYEE FROM PROJECT
    public async Task<bool> RemoveAsync(int employeeId, int projectId)
    {
        var employeeIdParam = new SqlParameter("@EmployeeId", employeeId);
        var projectIdParam = new SqlParameter("@ProjectId", projectId);
        
        var sql = "EXEC sp_RemoveEmployeeFromProject @EmployeeId, @ProjectId";
        
        var result = await _context.Database
            .SqlQueryRaw<int>(sql, employeeIdParam, projectIdParam)
            .ToListAsync();
        
        return result.FirstOrDefault() > 0;
    }
}
