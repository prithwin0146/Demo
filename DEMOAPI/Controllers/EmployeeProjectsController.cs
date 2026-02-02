using EmployeeApi.DTOs;
using EmployeeApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeProjectsController : ControllerBase
{
    private readonly IEmployeeProjectRepository _repository;

    public EmployeeProjectsController(IEmployeeProjectRepository repository)
    {
        _repository = repository;
    }
    //Get by ID
    [HttpGet("project/{projectId}")]
    public async Task<ActionResult<List<EmployeeProjectDto>>> GetByProject(int projectId)
    {
        var assignments = await _repository.GetByProjectIdAsync(projectId);
        return Ok(assignments);
    }
    //POST
    [HttpPost]
    public async Task<ActionResult<int>> Assign([FromBody] AssignEmployeeDto dto)
    {
        var id = await _repository.AssignAsync(dto);
        return Ok(id);
    }
    //DELETE
    [HttpDelete("{employeeId}/{projectId}")]
    public async Task<ActionResult> Remove(int employeeId, int projectId)
    {
        var result = await _repository.RemoveAsync(employeeId, projectId);
        if (!result)
            return NotFound();
        return NoContent();
    }
}
