using EmployeeApi.DTOs;
using EmployeeApi.Models;
using EmployeeApi.Repositories;

namespace EmployeeApi.Services;

public class EmployeeProjectService : IEmployeeProjectService
{
    private readonly IEmployeeProjectRepository _repository;

    public EmployeeProjectService(IEmployeeProjectRepository repository)
    {
        _repository = repository;
    }

    // GET BY PROJECT ID
    public List<EmployeeProjectDto> GetByProjectId(int projectId)
    {
        return _repository.GetByProjectId(projectId);
    }

    // ASSIGN EMPLOYEE TO PROJECT
    public int Assign(AssignEmployeeDto dto)
    {
        return _repository.Assign(dto);
    }

    // REMOVE EMPLOYEE FROM PROJECT
    public bool Remove(int employeeId, int projectId)
    {
        return _repository.Remove(employeeId, projectId);
    }

    // GET EMPLOYEE PROJECTS PAGED
    public async Task<PagedResponse<EmployeeProjectDto>> GetEmployeeProjectsPagedAsync(
        int projectId,
        PaginationRequest request)
    {
        var (employeeProjects, totalRecords) = await _repository.GetEmployeeProjectsPagedAsync(projectId, request);
        var totalPages = (int)Math.Ceiling(totalRecords / (double)request.PageSize);

        var employeeProjectDtos = employeeProjects.Select(ep => new EmployeeProjectDto
        {
            EmployeeProjectId = ep.EmployeeProjectId,
            EmployeeId = ep.EmployeeId,
            EmployeeName = ep.EmployeeName,
            ProjectId = ep.ProjectId,
            ProjectName = ep.ProjectName,
            AssignedDate = ep.AssignedDate,
            Role = ep.Role
        }).ToList();

        return new PagedResponse<EmployeeProjectDto>
        {
            Data = employeeProjectDtos,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRecords = totalRecords,
            TotalPages = totalPages
        };
    }
}
