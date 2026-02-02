using EmployeeApi.Models;

namespace EmployeeApi.Repositories;

public interface IProjectRepository
{
    Task<List<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(int id);
    Task<int> CreateAsync(Project project);
    Task<bool> UpdateAsync(int id, Project project);
}
