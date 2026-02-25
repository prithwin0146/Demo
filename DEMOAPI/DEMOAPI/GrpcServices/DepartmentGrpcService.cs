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

    public override Task<DepartmentResponse> CreateDepartment(CreateDepartmentRequest request, ServerCallContext context)
    {
        var dto = new DTOs.CreateDepartmentDto
        {
            DepartmentName = request.DepartmentName,
            Description = request.Description,
            ManagerId = request.ManagerId != 0 ? (int?)request.ManagerId : null
        };
        var createdId = _departmentService.Create(dto);
        var created = _departmentService.GetById(createdId);
        return Task.FromResult(MapToResponse(created));
    }

    public override Task<DepartmentResponse> UpdateDepartment(UpdateDepartmentRequest request, ServerCallContext context)
    {
        var dto = new DTOs.UpdateDepartmentDto
        {
            DepartmentName = request.DepartmentName,
            Description = request.Description,
            ManagerId = request.ManagerId != 0 ? (int?)request.ManagerId : null
        };
        var success = _departmentService.Update(request.DepartmentId, dto);
        var updated = _departmentService.GetById(request.DepartmentId);
        return Task.FromResult(MapToResponse(updated));
    }

    public override Task<DeleteDepartmentResponse> DeleteDepartment(DeleteDepartmentRequest request, ServerCallContext context)
    {
        var success = _departmentService.Delete(request.DepartmentId);
        return Task.FromResult(new DeleteDepartmentResponse
        {
            Success = success,
            Message = success ? "Department deleted successfully" : "Department deletion failed"
        });
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
