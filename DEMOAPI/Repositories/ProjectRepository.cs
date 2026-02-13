using EmployeeApi.Models;
using EmployeeApi.DTOs;
using EmployeeApi.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApi.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly TaskDbContext _context;

    public ProjectRepository(TaskDbContext context)
    {
        _context = context;
    }

    public List<Project> GetAll()
    {
        return _context.Projects.ToList();
    }

    public Project? GetById(int id)
    {
        return _context.Projects.Find(id);
    }

    public int Create(Project project)
    {
        _context.Projects.Add(project);
        _context.SaveChanges();
        return project.ProjectId;
    }

    public bool Update(int id, Project project)
    {
        var existingProject = _context.Projects.Find(id);
        if (existingProject == null) return false;

        existingProject.ProjectName = project.ProjectName;
        existingProject.Description = project.Description;
        existingProject.StartDate = project.StartDate;
        existingProject.EndDate = project.EndDate;
        existingProject.Status = project.Status;

        _context.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var project = _context.Projects.Find(id);
        if (project == null) return false;

        _context.Projects.Remove(project);
        _context.SaveChanges();
        return true;
    }

    public async Task<(List<ProjectPagedResult> Data, int TotalCount)> GetProjectsPagedAsync(PaginationRequest request)
    {
        var results = await _context.LoadStoredProc<ProjectPagedResult>(
            "sp_GetProjectsPaged",
            new SqlParameter("@PageNumber", request.PageNumber),
            new SqlParameter("@PageSize", request.PageSize),
            new SqlParameter("@SortBy", request.SortBy ?? "ProjectName"),
            new SqlParameter("@SortOrder", request.SortOrder == "DESC" ? "DESC" : "ASC"),
            new SqlParameter("@SearchTerm", (object?)request.SearchTerm ?? DBNull.Value),
            new SqlParameter("@HasEmployeesOnly", request.HasEmployeesOnly));

        var totalRecords = results.FirstOrDefault()?.TotalCount ?? 0;

        return (results.ToList(), totalRecords);
    }
}
