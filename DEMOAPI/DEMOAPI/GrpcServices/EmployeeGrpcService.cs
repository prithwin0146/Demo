using Grpc.Core;
using EmployeeApi.Grpc;
using EmployeeApi.Services;

namespace EmployeeApi.GrpcServices;

public class EmployeeGrpcService : EmployeeGrpc.EmployeeGrpcBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeeGrpcService(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    public override Task<EmployeeResponse> GetById(GetEmployeeRequest request, ServerCallContext context)
    {
        var emp = _employeeService.GetById(request.Id);
        if (emp == null)
            throw new RpcException(new Status(StatusCode.NotFound, $"Employee with ID {request.Id} not found"));

        return Task.FromResult(MapToResponse(emp));
    }
    public override Task<EmployeeResponse> CreateEmployee(CreateEmployeeRequest request, ServerCallContext context)
    {
        var dto = new DTOs.CreateEmployeeDto
        {
            Name = request.Name,
            Email = request.Email,
            JobRole = request.JobRole,
            SystemRole = request.SystemRole,
            DepartmentId = request.DepartmentId != 0 ? (int?)request.DepartmentId : null,
            Password = "GrpcDefaultPassword123" // Set a default or generate as needed
        };
        var created = _employeeService.Create(dto);
        return Task.FromResult(MapToResponse(created));
    }

    public override Task<EmployeeResponse> UpdateEmployee(UpdateEmployeeRequest request, ServerCallContext context)
    {
        var dto = new DTOs.UpdateEmployeeDto
        {
            Name = request.Name,
            Email = request.Email,
            JobRole = request.JobRole,
            SystemRole = request.SystemRole,
            DepartmentId = request.DepartmentId != 0 ? (int?)request.DepartmentId : null
        };
        var updated = _employeeService.Update(request.Id, dto);
        return Task.FromResult(MapToResponse(updated));
    }

    public override Task<DeleteEmployeeResponse> DeleteEmployee(DeleteEmployeeRequest request, ServerCallContext context)
    {
        var deleted = _employeeService.Delete(request.Id);
        var success = deleted != null;
        return Task.FromResult(new DeleteEmployeeResponse
        {
            Success = success,
            Message = success ? "Employee deleted successfully" : "Employee deletion failed"
        });
    }

    public override Task<EmployeeListResponse> GetAll(Empty request, ServerCallContext context)
    {
        var employees = _employeeService.GetAll();
        var response = new EmployeeListResponse();
        response.Employees.AddRange(employees.Select(MapToResponse));
        return Task.FromResult(response);
    }
   // public override Task<EmployeeResponse> 

    public static EmployeeResponse MapToResponse(DTOs.EmployeeDto dto)
    {
        var response = new EmployeeResponse
        {
            Id = dto.Id,
            Name = dto.Name,
            Email = dto.Email,
            JobRole = dto.JobRole,
            SystemRole = dto.SystemRole
        };

        if (dto.DepartmentId.HasValue)
            response.DepartmentId = dto.DepartmentId.Value;

        if (dto.DepartmentName != null)
            response.DepartmentName = dto.DepartmentName;

        return response;
    }
}
