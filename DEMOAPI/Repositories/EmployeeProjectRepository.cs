using EmployeeApi.Models;
using EmployeeApi.DTOs;
using Microsoft.EntityFrameworkCore;

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
        var results = await _context.EmployeeProjects
            .Where(ep => ep.ProjectId == projectId)
            .Join(_context.Employees,
                ep => ep.EmployeeId,
                e => e.Id,
                (ep, e) => new EmployeeProjectDto
                {
                    EmployeeId = ep.EmployeeId,
                    EmployeeName = e.Name,
                    ProjectId = ep.ProjectId,
                    Role = ep.Role,
                    AssignedDate = ep.AssignedDate
                })
            .ToListAsync();

        return results;
    }

    // ASSIGN EMPLOYEE TO PROJECT
    public async Task<int> AssignAsync(AssignEmployeeDto dto)
    {
        var employeeProject = new EmployeeProject
        {
            EmployeeId = dto.EmployeeId,
            ProjectId = dto.ProjectId,
            Role = dto.Role,
            AssignedDate = DateTime.Now
        };

        _context.EmployeeProjects.Add(employeeProject);
        await _context.SaveChangesAsync();

        return employeeProject.EmployeeProjectId;
    }

    // REMOVE EMPLOYEE FROM PROJECT
    public async Task<bool> RemoveAsync(int employeeId, int projectId)
    {
        var employeeProject = await _context.EmployeeProjects
            .FirstOrDefaultAsync(ep => ep.EmployeeId == employeeId && ep.ProjectId == projectId);

        if (employeeProject == null)
            return false;

        _context.EmployeeProjects.Remove(employeeProject);
        await _context.SaveChangesAsync();

        return true;
    }
}
