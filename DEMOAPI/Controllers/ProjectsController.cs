using EmployeeApi.DTOs;
using EmployeeApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly IUrlEncryptionService _urlEncryption;

    public ProjectsController(IProjectService projectService, IUrlEncryptionService urlEncryption)
    {
        _projectService = projectService;
        _urlEncryption = urlEncryption;
    }
    // GET
    [HttpGet]
    public ActionResult<List<ProjectDto>> GetAll()
    {
        var projects = _projectService.GetAll();
        return Ok(projects);
    }
    // GET BY ENCRYPTED ID
    [HttpGet("{encryptedId}")]
    public ActionResult<ProjectDto> GetById(string encryptedId)
    {
        int id;
        try { id = _urlEncryption.Decrypt(encryptedId); }
        catch { return BadRequest(new { message = "Invalid project ID" }); }

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
        return CreatedAtAction(nameof(GetById), new { encryptedId = _urlEncryption.Encrypt(projectId) }, new { projectId });
    }
    // PUT BY ENCRYPTED ID
    [HttpPut("{encryptedId}")]
    public ActionResult Update(string encryptedId, [FromBody] UpdateProjectDto updateDto)
    {
        int id;
        try { id = _urlEncryption.Decrypt(encryptedId); }
        catch { return BadRequest(new { message = "Invalid project ID" }); }

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

    // DELETE BY ENCRYPTED ID
    [HttpDelete("{encryptedId}")]
    public ActionResult Delete(string encryptedId)
    {
        int id;
        try { id = _urlEncryption.Decrypt(encryptedId); }
        catch { return BadRequest(new { message = "Invalid project ID" }); }

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

