using EmployeeApi.DTOs;
using EmployeeApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }
    // GET
    [HttpGet]
    public async Task<ActionResult<List<ProjectDto>>> GetAll()
    {
        var projects = await _projectService.GetAllAsync();
        return Ok(projects);
    }
    // GET BY ID
    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDto>> GetById(int id)
    {
        var project = await _projectService.GetByIdAsync(id);
        if (project == null)
        {
            return NotFound(new { message = "Project not found" });
        }
        return Ok(project);
    }
    // POST
    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] CreateProjectDto createDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var projectId = await _projectService.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = projectId }, new { projectId });
    }
    // PUT BY ID
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateProjectDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var success = await _projectService.UpdateAsync(id, updateDto);
        if (!success)
        {
            return NotFound(new { message = "Project not found" });
        }

        return NoContent();
    }

    // DELETE BY ID
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var success = await _projectService.DeleteAsync(id);
        if (!success)
        {
            return NotFound(new { message = "Project not found" });
        }

        return NoContent();
    }

    // GET PAGED
    [HttpGet("paged")]
    public async Task<ActionResult<PagedResponse<ProjectDto>>> GetPaged([FromQuery] PaginationRequest request)
    {
        var pagedResponse = await _projectService.GetProjectsPagedAsync(request);
        return Ok(pagedResponse);
    }
}

