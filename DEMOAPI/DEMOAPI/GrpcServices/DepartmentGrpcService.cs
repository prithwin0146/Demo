using Grpc.Core;
using EmployeeApi.Grpc;
using EmployeeApi.Services;
using Microsoft.Extensions.Logging;

namespace EmployeeApi.GrpcServices;

public class DepartmentGrpcService : DepartmentGrpc.DepartmentGrpcBase
{
    private readonly IDepartmentService _departmentService;
    private readonly ILogger<DepartmentGrpcService> _logger;

    public DepartmentGrpcService(IDepartmentService departmentService, ILogger<DepartmentGrpcService> logger)
    {
        _departmentService = departmentService;
        _logger = logger;
    }

    public override Task<DepartmentResponse> GetById(GetDepartmentRequest request, ServerCallContext context)
    {
        var dept = _departmentService.GetById(request.Id);
        if (dept == null)
        {
            _logger.LogWarning("Department with ID {DepartmentId} not found. Client: {Peer}",
                request.Id, context.Peer);
            throw new RpcException(new Status(StatusCode.NotFound, $"Department with ID {request.Id} not found"));
        }

        return Task.FromResult(MapToResponse(dept));
    }

    public override Task<DepartmentListResponse> GetAll(DepartmentEmpty request, ServerCallContext context)
    {
        var departments = _departmentService.GetAll();
        var response = new DepartmentListResponse();
        response.Departments.AddRange(departments.Select(MapToResponse));
        return Task.FromResult(response);
    }

    private static DepartmentResponse MapToResponse(DTOs.DepartmentDto dto)
    {
        var response = new DepartmentResponse
        {
            DepartmentId = dto.DepartmentId,
            DepartmentName = dto.DepartmentName,
            EmployeeCount = dto.EmployeeCount
        };

        if (dto.Description != null)
            response.Description = dto.Description;

        if (dto.ManagerId.HasValue)
            response.ManagerId = dto.ManagerId.Value;

        if (dto.ManagerName != null)
            response.ManagerName = dto.ManagerName;

        return response;
    }
}
