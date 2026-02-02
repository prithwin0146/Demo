using EmployeeApi.DTOs;
using EmployeeApi.Models;
using EmployeeApi.Repositories;

namespace EmployeeApi.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _repository;

    public ProjectService(IProjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ProjectDto>> GetAllAsync()
    {
        var projects = await _repository.GetAllAsync();
        return projects.Select(p => new ProjectDto
        {
            ProjectId = p.ProjectId,
            ProjectName = p.ProjectName,
            Description = p.Description,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            Status = p.Status
        }).ToList();
    }

    public async Task<ProjectDto?> GetByIdAsync(int id)
    {
        var project = await _repository.GetByIdAsync(id);
        if (project == null) return null;

        return new ProjectDto
        {
            ProjectId = project.ProjectId,
            ProjectName = project.ProjectName,
            Description = project.Description,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Status = project.Status
        };
    }

    public async Task<int> CreateAsync(CreateProjectDto createDto)
    {
        var project = new Project
        {
            ProjectName = createDto.ProjectName,
            Description = createDto.Description,
            StartDate = createDto.StartDate,
            EndDate = createDto.EndDate,
            Status = createDto.Status
        };

        return await _repository.CreateAsync(project);
    }

    public async Task<bool> UpdateAsync(int id, UpdateProjectDto updateDto)
    {
        var project = new Project
        {
            ProjectName = updateDto.ProjectName,
            Description = updateDto.Description,
            StartDate = updateDto.StartDate,
            EndDate = updateDto.EndDate,
            Status = updateDto.Status
        };

        return await _repository.UpdateAsync(id, project);
    }
}
