using EmployeeApi.DTOs;
using EmployeeApi.Models;
using EmployeeApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApi.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _repository;
    private readonly TaskDbContext _context;
    private readonly IUrlEncryptionService _urlEncryption;

    public DepartmentService(IDepartmentRepository repository, TaskDbContext context, IUrlEncryptionService urlEncryption)
    {
        _repository = repository;
        _context = context;
        _urlEncryption = urlEncryption;
    }
    // GET ALL
    public List<DepartmentDto> GetAll()
    {
        var departments = _repository.GetAll();
        var departmentDtos = new List<DepartmentDto>();

        foreach (var dept in departments)
        {
            var employeeCount = _repository.GetEmployeeCount(dept.DepartmentId);
            var managerName = dept.ManagerId.HasValue
                ? GetManagerName(dept.ManagerId.Value)
                : null;

            departmentDtos.Add(new DepartmentDto
            {
                DepartmentId = dept.DepartmentId,
                EncryptedId = _urlEncryption.Encrypt(dept.DepartmentId),
                DepartmentName = dept.DepartmentName,
                Description = dept.Description,
                ManagerId = dept.ManagerId,
                ManagerName = managerName,
                EmployeeCount = employeeCount
            });
        }

        return departmentDtos;
    }
    // GET BY ID
    public DepartmentDto? GetById(int id)
    {
        var dept = _repository.GetById(id);
        if (dept == null) return null;

        var employeeCount = _repository.GetEmployeeCount(dept.DepartmentId);
        var managerName = dept.ManagerId.HasValue
            ? GetManagerName(dept.ManagerId.Value)
            : null;

        return new DepartmentDto
        {
            DepartmentId = dept.DepartmentId,
            EncryptedId = _urlEncryption.Encrypt(dept.DepartmentId),
            DepartmentName = dept.DepartmentName,
            Description = dept.Description,
            ManagerId = dept.ManagerId,
            ManagerName = managerName,
            EmployeeCount = employeeCount
        };
    }
    // CREATE
    public int Create(CreateDepartmentDto createDto)
    {
        var department = new Department
        {
            DepartmentName = createDto.DepartmentName,
            Description = createDto.Description,
            ManagerId = createDto.ManagerId
        };

        return _repository.Create(department);
    }
    // UPDATE
    public bool Update(int id, UpdateDepartmentDto updateDto)
    {
        var department = new Department
        {
            DepartmentName = updateDto.DepartmentName,
            Description = updateDto.Description,
            ManagerId = updateDto.ManagerId
        };

        return _repository.Update(id, department);
    }
    // DELETE
    public bool Delete(int id)
    {
        var employeeCount = _repository.GetEmployeeCount(id);
        if (employeeCount > 0)
        {
            throw new InvalidOperationException("Cannot delete department with employees");
        }

        return _repository.Delete(id);
    }
    // PAGINATION
    public async Task<PagedResponse<DepartmentDto>> GetDepartmentsPagedAsync(PaginationRequest request)
    {
        var (departments, totalRecords) = await _repository.GetDepartmentsPagedAsync(request);
        var totalPages = (int)Math.Ceiling(totalRecords / (double)request.PageSize);

        var departmentDtos = departments.Select(d => new DepartmentDto
        {
            DepartmentId = d.DepartmentId,
            EncryptedId = _urlEncryption.Encrypt(d.DepartmentId),
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

    private string? GetManagerName(int managerId)
    {
        var employee = _context.Employees.Find(managerId);
        return employee?.Name;
    }
}
