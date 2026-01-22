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

        // GET employee by id
        [HttpGet("{id}")]
        public async Task<EmployeeDto> Get(int id)
        {
            return await _employeeService.GetByIdAsync(id);
        }

        // POST create employee
        [HttpPost]
        public async Task<EmployeeDto> Create(CreateEmployeeDto dto)
        {
            return await _employeeService.CreateAsync(dto);
        }

        // PUT update employee
        [HttpPut("{id}")]
        public async Task<EmployeeDto> Update(int id, CreateEmployeeDto dto)
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
