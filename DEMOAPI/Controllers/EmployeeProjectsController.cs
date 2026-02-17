using EmployeeApi.DTOs;
using EmployeeApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeProjectsController : ControllerBase
{
    private readonly IEmployeeProjectService _service;

    public EmployeeProjectsController(IEmployeeProjectService service)
    {
        _service = service;
    }

    // GET BY PROJECT ID (LEGACY - Returns all)
    [HttpGet("project/{projectId}")]
    public ActionResult<List<EmployeeProjectDto>> GetByProject(int projectId)
    {
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
    [HttpDelete("{employeeId}/{projectId}")]
    public ActionResult Remove(int employeeId, int projectId)
    {
        var result = _service.Remove(employeeId, projectId);
        if (!result)
            return NotFound();
        return NoContent();
    }
}
