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

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // GET all employees
        [HttpGet]
        public async Task<ActionResult<List<EmployeeDto>>> GetAll()
        {
            return await _employeeService.GetAllAsync();
        }

        // GET paginated employees with stored procedure
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResponse<EmployeeDto>>> GetPaged(
            [FromQuery] PaginationRequest request,
            [FromQuery] int? departmentId = null)
        {
            return await _employeeService.GetEmployeesPagedAsync(request, departmentId);
        }

        // GET employee by id
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> Get(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound(new { message = "Employee not found" });
            }
            return employee;
        }

        // POST create employee
        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> Create(CreateEmployeeDto dto)
        {
            try
            {
                var employee = await _employeeService.CreateAsync(dto);
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
        [HttpPut("{id}")]
        public async Task<ActionResult<EmployeeDto>> Update(int id, UpdateEmployeeDto dto)
        {
            try
            {
                var employee = await _employeeService.UpdateAsync(id, dto);
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
        [HttpDelete("{id}")]
        public async Task<ActionResult<EmployeeDto>> Delete(int id)
        {
            try
            {
                var employee = await _employeeService.DeleteAsync(id);
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
