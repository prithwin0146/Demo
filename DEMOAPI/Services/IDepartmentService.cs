using EmployeeApi.DTOs;

namespace EmployeeApi.Services;

public interface IDepartmentService
{
    List<DepartmentDto> GetAll();
    DepartmentDto? GetById(int id);
    int Create(CreateDepartmentDto createDto);
    bool Update(int id, UpdateDepartmentDto updateDto);
    bool Delete(int id);
    Task<PagedResponse<DepartmentDto>> GetDepartmentsPagedAsync(PaginationRequest request);
}
