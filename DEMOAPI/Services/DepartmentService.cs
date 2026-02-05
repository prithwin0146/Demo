using EmployeeApi.DTOs;
using EmployeeApi.Models;
using EmployeeApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApi.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _repository;
    private readonly TaskDbContext _context;

    public DepartmentService(IDepartmentRepository repository, TaskDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<List<DepartmentDto>> GetAllAsync()
    {
        var departments = await _repository.GetAllAsync();
        var departmentDtos = new List<DepartmentDto>();

        foreach (var dept in departments)
        {
            var employeeCount = await _repository.GetEmployeeCountAsync(dept.DepartmentId);
            var managerName = dept.ManagerId.HasValue
                ? await GetManagerNameAsync(dept.ManagerId.Value)
                : null;

            departmentDtos.Add(new DepartmentDto
            {
                DepartmentId = dept.DepartmentId,
                DepartmentName = dept.DepartmentName,
                Description = dept.Description,
                ManagerId = dept.ManagerId,
                ManagerName = managerName,
                EmployeeCount = employeeCount
            });
        }

        return departmentDtos;
    }

    public async Task<DepartmentDto?> GetByIdAsync(int id)
    {
        var dept = await _repository.GetByIdAsync(id);
        if (dept == null) return null;

        var employeeCount = await _repository.GetEmployeeCountAsync(dept.DepartmentId);
        var managerName = dept.ManagerId.HasValue
            ? await GetManagerNameAsync(dept.ManagerId.Value)
            : null;

        return new DepartmentDto
        {
            DepartmentId = dept.DepartmentId,
            DepartmentName = dept.DepartmentName,
            Description = dept.Description,
            ManagerId = dept.ManagerId,
            ManagerName = managerName,
            EmployeeCount = employeeCount
        };
    }

    public async Task<int> CreateAsync(CreateDepartmentDto createDto)
    {
        var department = new Department
        {
            DepartmentName = createDto.DepartmentName,
            Description = createDto.Description,
            ManagerId = createDto.ManagerId
        };

        return await _repository.CreateAsync(department);
    }

    public async Task<bool> UpdateAsync(int id, UpdateDepartmentDto updateDto)
    {
        var department = new Department
        {
            DepartmentName = updateDto.DepartmentName,
            Description = updateDto.Description,
            ManagerId = updateDto.ManagerId
        };

        return await _repository.UpdateAsync(id, department);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var employeeCount = await _repository.GetEmployeeCountAsync(id);
        if (employeeCount > 0)
        {
            throw new InvalidOperationException("Cannot delete department with employees");
        }

        return await _repository.DeleteAsync(id);
    }

    public async Task<PagedResponse<DepartmentDto>> GetDepartmentsPagedAsync(PaginationRequest request)
    {
        var (departments, totalRecords) = await _repository.GetDepartmentsPagedAsync(request);
        var totalPages = (int)Math.Ceiling(totalRecords / (double)request.PageSize);

        var departmentDtos = departments.Select(d => new DepartmentDto
        {
            DepartmentId = d.DepartmentId,
            DepartmentName = d.DepartmentName,
            Description = d.Description,
            ManagerId = d.ManagerId,
            ManagerName = d.ManagerName,
            EmployeeCount = d.EmployeeCount
        }).ToList();

        return new PagedResponse<DepartmentDto>
        {
            Data = departmentDtos,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRecords = totalRecords,
            TotalPages = totalPages
        };
    }

    private async Task<string?> GetManagerNameAsync(int managerId)
    {
        var employee = await _context.Employees.FindAsync(managerId);
        return employee?.Name;
    }
}
