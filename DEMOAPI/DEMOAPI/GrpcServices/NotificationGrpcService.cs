using Grpc.Core;
using EmployeeApi.Grpc;
using EmployeeApi.Services;

namespace EmployeeApi.GrpcServices;

public class NotificationGrpcService : NotificationGrpc.NotificationGrpcBase
{
    private readonly IEmailService _emailService;
    private readonly ILogger<NotificationGrpcService> _logger;

    public NotificationGrpcService(IEmailService emailService, ILogger<NotificationGrpcService> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public override Task<SendEmailResponse> SendEmail(SendEmailRequest request, ServerCallContext context)
    {
        try
        {
            _emailService.SendEmail(request.ToEmail, request.Subject, request.Body);

            _logger.LogInformation("Email sent successfully to {Email}", request.ToEmail);

            return Task.FromResult(new SendEmailResponse
            {
                Success = true,
                Message = "Email sent successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", request.ToEmail);

            return Task.FromResult(new SendEmailResponse
            {
                Success = false,
                Message = $"Failed to send email: {ex.Message}"
            });
        }
    }
}
