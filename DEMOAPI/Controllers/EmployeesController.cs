using EmployeeApi.DTOs;
using EmployeeApi.Services;
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
        public async Task<List<EmployeeDto>> GetAll()
        {
            return await _employeeService.GetAllAsync();
        }

        // GET paginated employees with stored procedure
        [HttpGet("paged")]
        public async Task<PagedResponse<EmployeeDto>> GetPaged([FromQuery] PaginationRequest request)
        {
            return await _employeeService.GetEmployeesPagedAsync(request);
        }

        // GET employee by id
        [HttpGet("{id}")]
        public async Task<EmployeeDto> Get(int id)
        {
            return await _employeeService.GetByIdAsync(id);
        }

        // POST create employee
        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> Create(CreateEmployeeDto dto)
        {
            // Validate password
            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest(new { message = "Password is required" });
            }

            if (dto.Password.Length < 8)
            {
                return BadRequest(new { message = "Password must be at least 8 characters long" });
            }

            try
            {
                var employee = await _employeeService.CreateAsync(dto);
                return Ok(employee);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT update employee
        [HttpPut("{id}")]
        public async Task<EmployeeDto> Update(int id, UpdateEmployeeDto dto)
        {
            return await _employeeService.UpdateAsync(id, dto);
        }

        // DELETE remove employee
        [HttpDelete("{id}")]
        public async Task<EmployeeDto> Delete(int id)
        {
            return await _employeeService.DeleteAsync(id);
        }
    }
}
