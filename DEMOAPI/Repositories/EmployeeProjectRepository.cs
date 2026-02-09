using EmployeeApi.Models;
using EmployeeApi.DTOs;
using Microsoft.EntityFrameworkCore;
using EmployeeApi.Extensions;
using Microsoft.Data.SqlClient;

namespace EmployeeApi.Repositories;

public interface IEmployeeProjectRepository
{
    List<EmployeeProjectDto> GetByProjectId(int projectId);
    int Assign(AssignEmployeeDto dto);
    bool Remove(int employeeId, int projectId);
    Task<(List<EmployeeProjectPagedResult> Data, int TotalCount)> GetEmployeeProjectsPagedAsync(
        int projectId,
        PaginationRequest request);
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

    // GET EMPLOYEE PROJECTS PAGED
    public async Task<(List<EmployeeProjectPagedResult> Data, int TotalCount)> GetEmployeeProjectsPagedAsync(
        int projectId,
        PaginationRequest request)
    {
        var results = await _context.LoadStoredProc<EmployeeProjectPagedResult>(
            "sp_GetEmployeeProjectsPaged",
            new SqlParameter("@ProjectId", projectId),
            new SqlParameter("@PageNumber", request.PageNumber),
            new SqlParameter("@PageSize", request.PageSize),
            new SqlParameter("@SortBy", request.SortBy ?? "AssignedDate"),
            new SqlParameter("@SortOrder", request.SortOrder == "DESC" ? "DESC" : "ASC"),
            new SqlParameter("@SearchTerm", (object?)request.SearchTerm ?? DBNull.Value));

        var totalRecords = results.FirstOrDefault()?.TotalCount ?? 0;

        return (results.ToList(), totalRecords);
    }
}
