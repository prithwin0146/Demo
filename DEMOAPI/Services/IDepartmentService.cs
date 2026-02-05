using EmployeeApi.DTOs;

namespace EmployeeApi.Services;

public interface IDepartmentService
{
    Task<List<DepartmentDto>> GetAllAsync();
    Task<DepartmentDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateDepartmentDto createDto);
    Task<bool> UpdateAsync(int id, UpdateDepartmentDto updateDto);
    Task<bool> DeleteAsync(int id);
    Task<PagedResponse<DepartmentDto>> GetDepartmentsPagedAsync(PaginationRequest request);
}
