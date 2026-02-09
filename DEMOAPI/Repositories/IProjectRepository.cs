using EmployeeApi.Models;
using EmployeeApi.DTOs;

namespace EmployeeApi.Repositories;

public interface IProjectRepository
{
    List<Project> GetAll();
    Project? GetById(int id);
    int Create(Project project);
    bool Update(int id, Project project);
    bool Delete(int id);
    Task<(List<ProjectPagedResult> Data, int TotalCount)> GetProjectsPagedAsync(PaginationRequest request);
}
