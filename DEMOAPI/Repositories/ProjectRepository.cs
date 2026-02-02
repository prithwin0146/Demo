using EmployeeApi.Models;
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
        return await _context.LoadStoredProc<Project>("GetAllProjects");
    }

    public async Task<Project?> GetByIdAsync(int id)
    {
        return await _context.LoadStoredProcSingle<Project>(
            "GetProjectById",
            new SqlParameter("@ProjectId", id));
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
}
