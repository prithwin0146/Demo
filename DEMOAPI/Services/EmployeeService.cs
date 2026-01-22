using EmployeeApi.DTOs;
using EmployeeApi.Models;
using EmployeeApi.Repositories;

namespace EmployeeApi.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;

    public EmployeeService(IEmployeeRepository repository)
    {
        _repository = repository;
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
            Role = emp.Role
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
            Role = emp.Role
        };
    }
    //CREATE
    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
    {
        var employee = new Employee
        {
            Name = dto.Name,
            Email = dto.Email,
            Role = dto.Role
        };

        var createdEmployee = await _repository.AddAsync(employee);

        return new EmployeeDto
        {
            Id = createdEmployee.Id,
            Name = createdEmployee.Name,
            Email = createdEmployee.Email,
            Role = createdEmployee.Role
        };
    }
    //UPDATE
    public async Task<EmployeeDto> UpdateAsync(int id, CreateEmployeeDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;

        existing.Name = dto.Name;
        existing.Email = dto.Email;
        existing.Role = dto.Role;

        var updatedEmployee = await _repository.UpdateAsync(existing);

        return new EmployeeDto
        {
            Id = updatedEmployee.Id,
            Name = updatedEmployee.Name,
            Email = updatedEmployee.Email,
            Role = updatedEmployee.Role
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
            Role = deletedEmployee.Role
        };
    }
}
