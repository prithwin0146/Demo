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
    // GET ALL  
    public List<ProjectDto> GetAll()
    {
        var projects = _repository.GetAll();
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
    // GET BY ID
    public ProjectDto? GetById(int id)
    {
        var project = _repository.GetById(id);
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
    // CREATE
    public int Create(CreateProjectDto createDto)
    {
        var project = new Project
        {
            ProjectName = createDto.ProjectName,
            Description = createDto.Description,
            StartDate = createDto.StartDate,
            EndDate = createDto.EndDate,
            Status = createDto.Status
        };

        return _repository.Create(project);
    }
    // UPDATE
    public bool Update(int id, UpdateProjectDto updateDto)
    {
        var project = new Project
        {
            ProjectName = updateDto.ProjectName,
            Description = updateDto.Description,
            StartDate = updateDto.StartDate,
            EndDate = updateDto.EndDate,
            Status = updateDto.Status
        };

        return _repository.Update(id, project);
    }
    // DELETE
    public bool Delete(int id)
    {
        return _repository.Delete(id);
    }
    // PAGINATION
    public async Task<PagedResponse<ProjectDto>> GetProjectsPagedAsync(PaginationRequest request)
    {
        var (projects, totalRecords) = await _repository.GetProjectsPagedAsync(request);
        var totalPages = (int)Math.Ceiling(totalRecords / (double)request.PageSize);

        var projectDtos = projects.Select(p => new ProjectDto
        {
            ProjectId = p.ProjectId,
            ProjectName = p.ProjectName,
            Description = p.Description,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            Status = p.Status,
            AssignedEmployees = p.AssignedEmployees,
            EmployeeNames = p.EmployeeNames
        }).ToList();

        return new PagedResponse<ProjectDto>
        {
            Data = projectDtos,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRecords = totalRecords,
            TotalPages = totalPages
        };
    }
}
