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

    public async Task<List<Project>> GetAllAsync()
    {
        return await _context.Projects.ToListAsync();
    }

    public async Task<Project?> GetByIdAsync(int id)
    {
        return await _context.Projects.FindAsync(id);
    }

    public async Task<int> CreateAsync(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return project.ProjectId;
    }

    public async Task<bool> UpdateAsync(int id, Project project)
    {
        var existingProject = await _context.Projects.FindAsync(id);
        if (existingProject == null) return false;

        existingProject.ProjectName = project.ProjectName;
        existingProject.Description = project.Description;
        existingProject.StartDate = project.StartDate;
        existingProject.EndDate = project.EndDate;
        existingProject.Status = project.Status;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null) return false;

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<(List<ProjectPagedResult> Data, int TotalCount)> GetProjectsPagedAsync(PaginationRequest request)
    {
        var results = await _context.LoadStoredProc<ProjectPagedResult>(
            "sp_GetProjectsPaged",
            new SqlParameter("@PageNumber", request.PageNumber),
            new SqlParameter("@PageSize", request.PageSize),
            new SqlParameter("@SortBy", request.SortBy ?? "ProjectName"),
            new SqlParameter("@SortOrder", request.Ascending ? "ASC" : "DESC"),
            new SqlParameter("@SearchTerm", (object?)request.SearchTerm ?? DBNull.Value));

        var totalRecords = results.FirstOrDefault()?.TotalCount ?? 0;

        return (results.ToList(), totalRecords);
    }
}
