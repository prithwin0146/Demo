using EmployeeApi.DTOs;
using EmployeeApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    // GET
    [HttpGet]
    public ActionResult<List<DepartmentDto>> GetAll()
    {
        var departments = _departmentService.GetAll();
        return Ok(departments);
    }

    // GET BY ID
    [HttpGet("{id}")]
    public ActionResult<DepartmentDto> GetById(int id)
    {
        var department = _departmentService.GetById(id);
        if (department == null)
        {
            return NotFound(new { message = "Department not found" });
        }
        return Ok(department);
    }

    // POST
    [HttpPost]
    public ActionResult<int> Create([FromBody] CreateDepartmentDto createDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var departmentId = _departmentService.Create(createDto);
        return CreatedAtAction(nameof(GetById), new { id = departmentId }, new { departmentId });
    }

    // PUT BY ID
    [HttpPut("{id}")]
    public ActionResult Update(int id, [FromBody] UpdateDepartmentDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var success = _departmentService.Update(id, updateDto);
        if (!success)
        {
            return NotFound(new { message = "Department not found" });
        }

        return NoContent();
    }

    // DELETE BY ID
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        try
        {
            var success = _departmentService.Delete(id);
            if (!success)
            {
                return NotFound(new { message = "Department not found" });
            }
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET PAGED
    [HttpGet("paged")]
    public async Task<ActionResult<PagedResponse<DepartmentDto>>> GetPaged([FromQuery] PaginationRequest request)
    {
        var pagedResponse = await _departmentService.GetDepartmentsPagedAsync(request);
        return Ok(pagedResponse);
    }
}
