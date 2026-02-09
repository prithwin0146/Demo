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
    public ActionResult<List<ProjectDto>> GetAll()
    {
        var projects = _projectService.GetAll();
        return Ok(projects);
    }
    // GET BY ID
    [HttpGet("{id}")]
    public ActionResult<ProjectDto> GetById(int id)
    {
        var project = _projectService.GetById(id);
        if (project == null)
        {
            return NotFound(new { message = "Project not found" });
        }
        return Ok(project);
    }
    // POST
    [HttpPost]
    public ActionResult<int> Create([FromBody] CreateProjectDto createDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var projectId = _projectService.Create(createDto);
        return CreatedAtAction(nameof(GetById), new { id = projectId }, new { projectId });
    }
    // PUT BY ID
    [HttpPut("{id}")]
    public ActionResult Update(int id, [FromBody] UpdateProjectDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var success = _projectService.Update(id, updateDto);
        if (!success)
        {
            return NotFound(new { message = "Project not found" });
        }

        return NoContent();
    }

    // DELETE BY ID
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var success = _projectService.Delete(id);
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

