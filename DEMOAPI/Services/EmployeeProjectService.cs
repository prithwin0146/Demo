using EmployeeApi.DTOs;
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
}
