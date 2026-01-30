using EmployeeApi.DTOs;
using EmployeeApi.Models;
using EmployeeApi.Repositories;

namespace EmployeeApi.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly TaskDbContext _context;

    public EmployeeService(IEmployeeRepository repository, IUserRepository userRepository, TaskDbContext context)
    {
        _repository = repository;
        _userRepository = userRepository;
        _context = context;
    }
    //GET ALL
    public async Task<List<EmployeeDto>> GetAllAsync()
    {
        var employees = await _repository.GetAllAsync();

        return employees.Select(emp => new EmployeeDto
        {
            Id = emp.Id,
            Name = emp.Name,
            Email = emp.Email,
            JobRole = emp.JobRole,
            SystemRole = emp.Role ?? "Employee"
        }).ToList();
    }
    //GET BY ID
    public async Task<EmployeeDto> GetByIdAsync(int id)
    {
        var emp = await _repository.GetByIdAsync(id);
        if (emp == null) return null;

        return new EmployeeDto
        {
            Id = emp.Id,
            Name = emp.Name,
            Email = emp.Email,
            JobRole = emp.JobRole,
            SystemRole = emp.Role ?? "Employee"
        };
    }
    //CREATE
    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
    {
        // Check if user with this email already exists
        var existingUser = _userRepository.Find(u => u.Email == dto.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("A user with this email already exists.");
        }

        // Create employee record
        var employee = new Employee
        {
            Name = dto.Name,
            Email = dto.Email,
            JobRole = dto.JobRole,
            Role = dto.SystemRole ?? "Employee"
        };

        // Use stored procedure to create employee with password
        var createdEmployee = await _repository.AddWithPasswordAsync(employee, dto.Password);

        return new EmployeeDto
        {
            Id = createdEmployee.Id,
            Name = createdEmployee.Name,
            Email = createdEmployee.Email,
            JobRole = createdEmployee.JobRole,
            SystemRole = createdEmployee.Role ?? "Employee"
        };
    }
    //UPDATE
    public async Task<EmployeeDto> UpdateAsync(int id, UpdateEmployeeDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;

        existing.Name = dto.Name;
        existing.Email = dto.Email;
        existing.JobRole = dto.JobRole;
        existing.Role = dto.SystemRole ?? "Employee";

        var updatedEmployee = await _repository.UpdateAsync(existing);

        return new EmployeeDto
        {
            Id = updatedEmployee.Id,
            Name = updatedEmployee.Name,
            Email = updatedEmployee.Email,
            JobRole = updatedEmployee.JobRole,
            SystemRole = updatedEmployee.Role ?? "Employee"
        };
    }
    //DELETE
    public async Task<EmployeeDto> DeleteAsync(int id)
    {
        var emp = await _repository.GetByIdAsync(id);
        if (emp == null) return null;

        var deletedEmployee = await _repository.DeleteAsync(emp);

        return new EmployeeDto
        {
            Id = deletedEmployee.Id,
            Name = deletedEmployee.Name,
            Email = deletedEmployee.Email,
            JobRole = deletedEmployee.JobRole,
            SystemRole = deletedEmployee.Role ?? "Employee"
        };
    }

    //GET PAGED WITH STORED PROCEDURE
    public async Task<PagedResponse<EmployeeDto>> GetEmployeesPagedAsync(PaginationRequest request)
    {
        return await _repository.GetEmployeesPagedAsync(request);
    }
}
