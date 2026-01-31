using EmployeeApi.DTOs;
using EmployeeApi.Models;
using EmployeeApi.Repositories;
using EmployeeApi.Exceptions;

namespace EmployeeApi.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordValidator _passwordValidator;
    private readonly IEmployeeMapper _mapper;

    public EmployeeService(
        IEmployeeRepository repository, 
        IUserRepository userRepository,
        IPasswordValidator passwordValidator,
        IEmployeeMapper mapper)
    {
        _repository = repository;
        _userRepository = userRepository;
        _passwordValidator = passwordValidator;
        _mapper = mapper;
    }
    //GET ALL
    public async Task<List<EmployeeDto>> GetAllAsync()
    {
        var employees = await _repository.GetAllAsync();
        return _mapper.ToDto(employees);
    }
    //GET BY ID
    public async Task<EmployeeDto> GetByIdAsync(int id)
    {
        var emp = await _repository.GetByIdAsync(id);
        if (emp == null) return null;

        return _mapper.ToDto(emp);
    }
    //CREATE
    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
    {
        var (isValid, errorMessage) = _passwordValidator.Validate(dto.Password);
        if (!isValid)
        {
            throw new InvalidPasswordException(errorMessage);
        }

        if (await _userRepository.ExistsWithEmailAsync(dto.Email))
        {
            throw new DuplicateEmailException(dto.Email);
        }

        var employee = new Employee
        {
            Name = dto.Name,
            Email = dto.Email,
            JobRole = dto.JobRole,
            Role = dto.SystemRole ?? "Employee"
        };

        var createdEmployee = await _repository.AddWithPasswordAsync(employee, dto.Password);
        return _mapper.ToDto(createdEmployee);
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
        return _mapper.ToDto(updatedEmployee);
    }
    //DELETE
    public async Task<EmployeeDto> DeleteAsync(int id)
    {
        var emp = await _repository.GetByIdAsync(id);
        if (emp == null) return null;

        var deletedEmployee = await _repository.DeleteAsync(emp);
        return _mapper.ToDto(deletedEmployee);
    }

    //GET PAGED WITH STORED PROCEDURE
    public async Task<PagedResponse<EmployeeDto>> GetEmployeesPagedAsync(PaginationRequest request)
    {
        var (employees, totalRecords) = await _repository.GetEmployeesPagedAsync(request);
        var totalPages = (int)Math.Ceiling(totalRecords / (double)request.PageSize);

        return new PagedResponse<EmployeeDto>
        {
            Data = _mapper.ToDto(employees),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRecords = totalRecords,
            TotalPages = totalPages
        };
    }
}
