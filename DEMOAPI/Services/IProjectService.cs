using EmployeeApi.DTOs;

namespace EmployeeApi.Services;

public interface IProjectService
{
    List<ProjectDto> GetAll();
    ProjectDto? GetById(int id);
    int Create(CreateProjectDto createDto);
    bool Update(int id, UpdateProjectDto updateDto);
    bool Delete(int id);
    Task<PagedResponse<ProjectDto>> GetProjectsPagedAsync(PaginationRequest request);
}
