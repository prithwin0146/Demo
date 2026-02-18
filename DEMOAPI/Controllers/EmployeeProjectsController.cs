using EmployeeApi.DTOs;
using EmployeeApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeProjectsController : ControllerBase
{
    private readonly IEmployeeProjectService _service;
    private readonly IUrlEncryptionService _urlEncryption;

    public EmployeeProjectsController(IEmployeeProjectService service, IUrlEncryptionService urlEncryption)
    {
        _service = service;
        _urlEncryption = urlEncryption;
    }

    // GET BY PROJECT ENCRYPTED ID
    [HttpGet("project/{encryptedProjectId}")]
    public ActionResult<List<EmployeeProjectDto>> GetByProject(string encryptedProjectId)
    {
        int projectId;
        try { projectId = _urlEncryption.Decrypt(encryptedProjectId); }
        catch { return BadRequest(new { message = "Invalid project ID" }); }

        var assignments = _service.GetByProjectId(projectId);
        return Ok(assignments);
    }

    // POST
    [HttpPost]
    public ActionResult<int> Assign([FromBody] AssignEmployeeDto dto)
    {
        var id = _service.Assign(dto);
        if (id == -1)
            return Conflict(new { message = "Employee is already assigned to this project" });
        return Ok(id);
    }

    // DELETE
    [HttpDelete("{encryptedEmployeeId}/{encryptedProjectId}")]
    public ActionResult Remove(string encryptedEmployeeId, string encryptedProjectId)
    {
        int employeeId, projectId;
        try
        {
            employeeId = _urlEncryption.Decrypt(encryptedEmployeeId);
            projectId = _urlEncryption.Decrypt(encryptedProjectId);
        }
        catch { return BadRequest(new { message = "Invalid ID" }); }

        var result = _service.Remove(employeeId, projectId);
        if (!result)
            return NotFound();
        return NoContent();
    }
}
