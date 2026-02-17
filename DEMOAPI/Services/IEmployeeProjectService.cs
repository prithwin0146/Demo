using EmployeeApi.DTOs;

namespace EmployeeApi.Services;

public interface IEmployeeProjectService
{
    List<EmployeeProjectDto> GetByProjectId(int projectId);
    int Assign(AssignEmployeeDto dto);
    bool Remove(int employeeId, int projectId);
}
