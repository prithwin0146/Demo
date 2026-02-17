using EmployeeApi.Models;
using EmployeeApi.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApi.Repositories;

public interface IEmployeeProjectRepository
{
    List<EmployeeProjectDto> GetByProjectId(int projectId);
    int Assign(AssignEmployeeDto dto);
    bool Remove(int employeeId, int projectId);
}

public class EmployeeProjectRepository : IEmployeeProjectRepository
{
    private readonly TaskDbContext _context;

    public EmployeeProjectRepository(TaskDbContext context)
    {
        _context = context;
    }

    // GET BY PROJECT ID
    public List<EmployeeProjectDto> GetByProjectId(int projectId)
    {
        var results = _context.EmployeeProjects
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
            .ToList();

        return results;
    }

    // ASSIGN EMPLOYEE TO PROJECT
    public int Assign(AssignEmployeeDto dto)
    {
        var exists = _context.EmployeeProjects
            .Any(ep => ep.EmployeeId == dto.EmployeeId && ep.ProjectId == dto.ProjectId);

        if (exists)
            return -1;

        var employeeProject = new EmployeeProject
        {
            EmployeeId = dto.EmployeeId,
            ProjectId = dto.ProjectId,
            Role = dto.Role,
            AssignedDate = DateTime.Now
        };

        _context.EmployeeProjects.Add(employeeProject);
        _context.SaveChanges();

        return employeeProject.EmployeeProjectId;
    }

    // REMOVE EMPLOYEE FROM PROJECT
    public bool Remove(int employeeId, int projectId)
    {
        var employeeProject = _context.EmployeeProjects
            .FirstOrDefault(ep => ep.EmployeeId == employeeId && ep.ProjectId == projectId);

        if (employeeProject == null)
            return false;

        _context.EmployeeProjects.Remove(employeeProject);
        _context.SaveChanges();

        return true;
    }
}
