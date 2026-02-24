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
    private readonly IEmailService _emailService;

    private static readonly HashSet<string> ValidRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "Admin", "HR", "Manager", "Employee"
    };

    private static string NormalizeRole(string? role)
    {
        if (string.IsNullOrWhiteSpace(role)) return "Employee";

        // Match against valid roles (case-insensitive) and return the canonical PascalCase form
        foreach (var validRole in ValidRoles)
        {
            if (string.Equals(role, validRole, StringComparison.OrdinalIgnoreCase))
                return validRole;
        }

        throw new ArgumentException($"Invalid role '{role}'. Valid roles are: Admin, HR, Manager, Employee.");
    }

    public EmployeeService(
        IEmployeeRepository repository, 
        IUserRepository userRepository,
        IPasswordValidator passwordValidator,
        IEmployeeMapper mapper,
        IEmailService emailService)
    {
        _repository = repository;
        _userRepository = userRepository;
        _passwordValidator = passwordValidator;
        _mapper = mapper;
        _emailService = emailService;
    }
    //GET ALL
    public List<EmployeeDto> GetAll()
    {
        var employees = _repository.GetAll();
        return _mapper.ToDto(employees);
    }
    //GET BY ID
    public EmployeeDto GetById(int id)
    {
        var emp = _repository.GetById(id);
        if (emp == null) return null;

        return _mapper.ToDto(emp);
    }
    //CREATE
    public EmployeeDto Create(CreateEmployeeDto dto)
    {
        var (isValid, errorMessage) = _passwordValidator.Validate(dto.Password);
        if (!isValid)
        {
            throw new InvalidPasswordException(errorMessage);
        }

        if (_userRepository.ExistsWithEmail(dto.Email))
        {
            throw new DuplicateEmailException(dto.Email);
        }

        if (_repository.ExistsByEmail(dto.Email))
        {
            throw new DuplicateEmailException(dto.Email);
        }

        var normalizedRole = NormalizeRole(dto.SystemRole);

        var employee = new Employee
        {
            Name = dto.Name,
            Email = dto.Email,
            JobRole = dto.JobRole,
            Role = normalizedRole,
            DepartmentId = dto.DepartmentId
        };

        var createdEmployee = _repository.AddWithPassword(employee, dto.Password);

        _ = Task.Run(() =>
        {
            try
            {
                _emailService.SendEmail(
                    createdEmployee.Email,
                    "Welcome to the Team!",
                    $@"<h3>Hello {createdEmployee.Name},</h3>
                    <p>Welcome aboard! Your account has been created in the Employee Management System.</p>
                    <p><strong>Role:</strong> {createdEmployee.JobRole}<br/>
                    <strong>Email:</strong> {createdEmployee.Email}</p>
                    <p>Please log in using your registered email and password.</p>
                    <br/>
                    <p>Regards,<br/>Human Resource</p>");
            }
            catch { }
        });

        return _mapper.ToDto(createdEmployee);
    }
    //UPDATE
    public EmployeeDto Update(int id, UpdateEmployeeDto dto)
    {
        var existing = _repository.GetById(id);
        if (existing == null) return null;

        var oldEmail = existing.Email;
        var normalizedRole = NormalizeRole(dto.SystemRole);

        existing.Name = dto.Name;
        existing.Email = dto.Email;
        existing.JobRole = dto.JobRole;
        existing.Role = normalizedRole;
        existing.DepartmentId = dto.DepartmentId;

        var updatedEmployee = _repository.Update(existing);

        // Sync role (and email if changed) to User record so JWT reflects the update on next login
        var user = _userRepository.GetByEmail(oldEmail);
        if (user != null)
        {
            user.Role = normalizedRole;
            if (!string.Equals(oldEmail, dto.Email, StringComparison.OrdinalIgnoreCase))
            {
                user.Email = dto.Email;
            }
            _userRepository.Update(user);
            _userRepository.SaveChangesAsync().Wait();
        }

        return _mapper.ToDto(updatedEmployee);
    }
    //DELETE
    public EmployeeDto Delete(int id)
    {
        var emp = _repository.GetById(id);
        if (emp == null) return null;

        var email = emp.Email;
        var name = emp.Name;

        var deletedEmployee = _repository.Delete(emp);

        var user = _userRepository.GetByEmail(email);
        if (user != null)
        {
            _userRepository.Delete(user);
            _userRepository.SaveChangesAsync().Wait();
        }

        _ = Task.Run(() =>
        {
            try
            {
                _emailService.SendEmail(
                    email,
                    "Account Removed - Employee Management System",
                    $@"<h3>Hello {name},</h3>
                    <p>Your account has been removed from the Employee Management System.</p>
                    <p>If you believe this was done in error, please contact your manager or the HR department.</p>
                    <br/>
                    <p>Regards,<br/>Human Resource</p>");
            }
            catch { }
        });

        return _mapper.ToDto(deletedEmployee);
    }

    //GET PAGED WITH STORED PROCEDURE
    public async Task<PagedResponse<EmployeeDto>> GetEmployeesPagedAsync(PaginationRequest request, int? departmentId = null, string? jobRole = null, string? systemRole = null, int? projectId = null)
    {
        var (employees, totalRecords) = await _repository.GetEmployeesPagedAsync(request, departmentId, jobRole, systemRole, projectId);
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
