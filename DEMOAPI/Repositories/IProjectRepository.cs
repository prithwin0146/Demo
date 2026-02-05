using EmployeeApi.Models;
using EmployeeApi.DTOs;

namespace EmployeeApi.Repositories;

public interface IProjectRepository
{
    Task<List<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(int id);
    Task<int> CreateAsync(Project project);
    Task<bool> UpdateAsync(int id, Project project);
    Task<bool> DeleteAsync(int id);
    Task<(List<ProjectPagedResult> Data, int TotalCount)> GetProjectsPagedAsync(PaginationRequest request);
}
