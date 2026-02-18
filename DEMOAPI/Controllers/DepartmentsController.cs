using EmployeeApi.DTOs;
using EmployeeApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{           
    private readonly IDepartmentService _departmentService;
    private readonly IUrlEncryptionService _urlEncryption;

    public DepartmentsController(IDepartmentService departmentService, IUrlEncryptionService urlEncryption)
    {
        _departmentService = departmentService;
        _urlEncryption = urlEncryption;
    }

    // GET
    [HttpGet]
    public ActionResult<List<DepartmentDto>> GetAll()
    {
        var departments = _departmentService.GetAll();
        return Ok(departments);
    }

    // GET BY ENCRYPTED ID
    [HttpGet("{encryptedId}")]
    public ActionResult<DepartmentDto> GetById(string encryptedId)
    {
        int id;
        try { id = _urlEncryption.Decrypt(encryptedId); }
        catch { return BadRequest(new { message = "Invalid department ID" }); }

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
        return CreatedAtAction(nameof(GetById), new { encryptedId = _urlEncryption.Encrypt(departmentId) }, new { departmentId });
    }

    // PUT BY ENCRYPTED ID
    [HttpPut("{encryptedId}")]
    public ActionResult Update(string encryptedId, [FromBody] UpdateDepartmentDto updateDto)
    {
        int id;
        try { id = _urlEncryption.Decrypt(encryptedId); }
        catch { return BadRequest(new { message = "Invalid department ID" }); }

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

    // DELETE BY ENCRYPTED ID
    [HttpDelete("{encryptedId}")]
    public ActionResult Delete(string encryptedId)
    {
        int id;
        try { id = _urlEncryption.Decrypt(encryptedId); }
        catch { return BadRequest(new { message = "Invalid department ID" }); }

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
