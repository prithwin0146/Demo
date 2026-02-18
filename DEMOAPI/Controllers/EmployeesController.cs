using EmployeeApi.DTOs;
using EmployeeApi.Services;
using EmployeeApi.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IUrlEncryptionService _urlEncryption;

        public EmployeesController(IEmployeeService employeeService, IUrlEncryptionService urlEncryption)
        {
            _employeeService = employeeService;
            _urlEncryption = urlEncryption;
        }

        // GET all employees
        [HttpGet]
        public ActionResult<List<EmployeeDto>> GetAll()
        {
            return _employeeService.GetAll();
        }

        // GET paginated employees with stored procedure
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResponse<EmployeeDto>>> GetPaged(
            [FromQuery] PaginationRequest request,
            [FromQuery] int? departmentId = null,
            [FromQuery] string? jobRole = null,
            [FromQuery] string? systemRole = null,
            [FromQuery] int? projectId = null)
        {
            return await _employeeService.GetEmployeesPagedAsync(request, departmentId, jobRole, systemRole, projectId);
        }

        // GET employee by encrypted id
        [HttpGet("{encryptedId}")]
        public ActionResult<EmployeeDto> Get(string encryptedId)
        {
            int id;
            try { id = _urlEncryption.Decrypt(encryptedId); }
            catch { return BadRequest(new { message = "Invalid employee ID" }); }

            var employee = _employeeService.GetById(id);
            if (employee == null)
            {
                return NotFound(new { message = "Employee not found" });
            }
            return employee;
        }

        // POST create employee
        [HttpPost]
        public ActionResult<EmployeeDto> Create(CreateEmployeeDto dto)
        {
            try
            {
                var employee = _employeeService.Create(dto);
                return Ok(employee);
            }
            catch (InvalidPasswordException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (DuplicateEmailException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT update employee
        [HttpPut("{encryptedId}")]
        public ActionResult<EmployeeDto> Update(string encryptedId, UpdateEmployeeDto dto)
        {
            int id;
            try { id = _urlEncryption.Decrypt(encryptedId); }
            catch { return BadRequest(new { message = "Invalid employee ID" }); }

            try
            {
                var employee = _employeeService.Update(id, dto);
                if (employee == null)
                {
                    return NotFound(new { message = "Employee not found" });
                }
                return Ok(employee);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE remove employee
        [HttpDelete("{encryptedId}")]
        public ActionResult<EmployeeDto> Delete(string encryptedId)
        {
            int id;
            try { id = _urlEncryption.Decrypt(encryptedId); }
            catch { return BadRequest(new { message = "Invalid employee ID" }); }

            try
            {
                var employee = _employeeService.Delete(id);
                if (employee == null)
                {
                    return NotFound(new { message = "Employee not found" });
                }
                return Ok(employee);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
