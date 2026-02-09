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

    // GET PAGED BY PROJECT ID (NEW - Returns paginated)
    [HttpGet("project/{projectId}/paged")]
    public async Task<ActionResult<PagedResponse<EmployeeProjectDto>>> GetByProjectPaged(
        int projectId,
        [FromQuery] PaginationRequest request)
    {
        var pagedResponse = await _service.GetEmployeeProjectsPagedAsync(projectId, request);
        return Ok(pagedResponse);
    }

    // POST
    [HttpPost]
    public ActionResult<int> Assign([FromBody] AssignEmployeeDto dto)
    {
        var id = _service.Assign(dto);
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
