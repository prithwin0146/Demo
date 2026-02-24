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

    public override Task<EmployeeListResponse> GetAll(Empty request, ServerCallContext context)
    {
        var employees = _employeeService.GetAll();
        var response = new EmployeeListResponse();
        response.Employees.AddRange(employees.Select(MapToResponse));
        return Task.FromResult(response);
    }

    private static EmployeeResponse MapToResponse(DTOs.EmployeeDto dto)
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
