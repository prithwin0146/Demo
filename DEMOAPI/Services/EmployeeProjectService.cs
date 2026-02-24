using EmployeeApi.DTOs;
using EmployeeApi.Repositories;

namespace EmployeeApi.Services;

public class EmployeeProjectService : IEmployeeProjectService
{
    private readonly IEmployeeProjectRepository _repository;
    private readonly IUrlEncryptionService _urlEncryption;
    private readonly IEmailService _emailService;
    private readonly IEmployeeService _employeeService;
    private readonly IProjectService _projectService;

    public EmployeeProjectService(
        IEmployeeProjectRepository repository,
        IUrlEncryptionService urlEncryption,
        IEmailService emailService,
        IEmployeeService employeeService,
        IProjectService projectService)
    {
        _repository = repository;
        _urlEncryption = urlEncryption;
        _emailService = emailService;
        _employeeService = employeeService;
        _projectService = projectService;
    }

    // GET BY PROJECT ID
    public List<EmployeeProjectDto> GetByProjectId(int projectId)
    {
        var results = _repository.GetByProjectId(projectId);
        foreach (var dto in results)
        {
            dto.EncryptedEmployeeId = _urlEncryption.Encrypt(dto.EmployeeId);
            dto.EncryptedProjectId = _urlEncryption.Encrypt(dto.ProjectId);
        }
        return results;
    }

    // ASSIGN EMPLOYEE TO PROJECT
    public int Assign(AssignEmployeeDto dto)
    {
        var id = _repository.Assign(dto);
        if (id == -1)
            return id;

        var employee = _employeeService.GetById(dto.EmployeeId);
        var project = _projectService.GetById(dto.ProjectId);

        if (employee != null && project != null)
        {
            var toEmail = employee.Email;
            var subject = $"New Project Assignment: {project.ProjectName}";
            var body = $@"
                <h3>Hello {employee.Name},</h3>
                <p>You have been assigned to the project <strong>{project.ProjectName}</strong> as <strong>{dto.Role ?? "Team Member"}</strong>.</p>
                <p>Please check the project details in the Employee Management System.</p>
                <br/>
                <p>Regards,<br/>Human Resource</p>";

            _ = Task.Run(() =>
            {
                try { _emailService.SendEmail(toEmail, subject, body); }
                catch { 
                
                }
            });
        }

        return id;
    }

    // REMOVE EMPLOYEE FROM PROJECT
    public bool Remove(int employeeId, int projectId)
    {
        var removed = _repository.Remove(employeeId, projectId);
        if (!removed)
            return false;

        var employee = _employeeService.GetById(employeeId);
        var project = _projectService.GetById(projectId);

        if (employee != null && project != null)
        {
            var toEmail = employee.Email;
            var subject = $"Project Unassignment: {project.ProjectName}";
            var body = $@"
                <h3>Hello {employee.Name},</h3>
                <p>You have been unassigned from the project <strong>{project.ProjectName}</strong>.</p>
                <p>If you believe this was a mistake, please contact your manager or HR.</p>
                <br/>
                <p>Regards,<br/>Human Resource</p>";

            _ = Task.Run(() =>
            {
                try { _emailService.SendEmail(toEmail, subject, body); }
                catch {

                }
            });
        }

        return true;
    }
}
