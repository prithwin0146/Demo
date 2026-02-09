using EmployeeApi.DTOs;

namespace EmployeeApi.Services;

public interface IEmployeeService
{
    List<EmployeeDto> GetAll();
    EmployeeDto GetById(int id);
    EmployeeDto Create(CreateEmployeeDto dto);
    EmployeeDto Update(int id, UpdateEmployeeDto dto);
    EmployeeDto Delete(int id);
    Task<PagedResponse<EmployeeDto>> GetEmployeesPagedAsync(PaginationRequest request, int? departmentId = null, string? jobRole = null, string? systemRole = null);
}
