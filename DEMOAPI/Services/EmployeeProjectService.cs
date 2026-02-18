using EmployeeApi.DTOs;
using EmployeeApi.Repositories;

namespace EmployeeApi.Services;

public class EmployeeProjectService : IEmployeeProjectService
{
    private readonly IEmployeeProjectRepository _repository;
    private readonly IUrlEncryptionService _urlEncryption;

    public EmployeeProjectService(IEmployeeProjectRepository repository, IUrlEncryptionService urlEncryption)
    {
        _repository = repository;
        _urlEncryption = urlEncryption;
    }

    // GET BY PROJECT ID
    public List<EmployeeProjectDto> GetByProjectId(int projectId)
    {
        var results = _repository.GetByProjectId(projectId);
        foreach (var dto in results)
        {
            dto.EncryptedEmployeeId = _urlEncryption.Encrypt(dto.EmployeeId);
            dto.EncryptedProjectId = _urlEncryption.Encrypt(dto.ProjectId);
        }
        return results;
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
