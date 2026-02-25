using Grpc.Net.Client;
using EmployeeApi.Grpc;

var channel = GrpcChannel.ForAddress("http://localhost:5128");

var employeeClient = new EmployeeGrpc.EmployeeGrpcClient(channel);
var departmentClient = new DepartmentGrpc.DepartmentGrpcClient(channel);
var notificationClient = new NotificationGrpc.NotificationGrpcClient(channel);

Console.WriteLine("=== gRPC Client ===");
Console.WriteLine("Connected to http://localhost:5128\n");

while (true)
{
    Console.WriteLine("Choose a service:");
    Console.WriteLine("  1. Employees - Get All");
    Console.WriteLine("  2. Employees - Get By ID");
    Console.WriteLine("  3. Employees - Create");
    Console.WriteLine("  4. Employees - Update");
    Console.WriteLine("  5. Employees - Delete");
    Console.WriteLine("  6. Departments - Get All");
    Console.WriteLine("  7. Departments - Get By ID");
    Console.WriteLine("  8. Departments - Create");
    Console.WriteLine("  9. Departments - Update");
    Console.WriteLine(" 10. Departments - Delete");
    Console.WriteLine(" 11. Notification - Send Email");
    Console.WriteLine("  0. Exit");
    Console.Write("\n> ");

    var choice = Console.ReadLine()?.Trim();

    try
    {
        switch (choice)
        {
            case "1": await GetAllEmployees(employeeClient); break;
            case "2": await GetEmployeeById(employeeClient); break;
            case "3": await CreateEmployee(employeeClient); break;
            case "4": await UpdateEmployee(employeeClient); break;
            case "5": await DeleteEmployee(employeeClient); break;
            case "6": await GetAllDepartments(departmentClient); break;
            case "7": await GetDepartmentById(departmentClient); break;
            case "8": await CreateDepartment(departmentClient); break;
            case "9": await UpdateDepartment(departmentClient); break;
            case "10": await DeleteDepartment(departmentClient); break;
            case "11": await SendEmail(notificationClient); break;
            case "0": Console.WriteLine("Bye!"); return;
            default: Console.WriteLine("Invalid choice.\n"); break;
        }
    }
    catch (Grpc.Core.RpcException ex)
    {
        Console.WriteLine($"\n  gRPC Error: {ex.Status.StatusCode} - {ex.Status.Detail}\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n  Error: {ex.Message}\n");
    }
}

static async Task GetAllEmployees(EmployeeGrpc.EmployeeGrpcClient client)
{
    var response = await client.GetAllAsync(new Empty());

    Console.WriteLine($"\n  Found {response.Employees.Count} employee(s):");
    foreach (var emp in response.Employees)
    {
        Console.WriteLine($"    [{emp.Id}] {emp.Name} | {emp.Email} | {emp.JobRole} | Dept: {(emp.HasDepartmentName ? emp.DepartmentName : "N/A")}");
    }
    Console.WriteLine();
}

static async Task GetEmployeeById(EmployeeGrpc.EmployeeGrpcClient client)
{
    Console.Write("  Enter Employee ID: ");
    if (!int.TryParse(Console.ReadLine(), out var id))
    {
        Console.WriteLine("  Invalid ID.\n");
        return;
    }

    var emp = await client.GetByIdAsync(new GetEmployeeRequest { Id = id });
    Console.WriteLine($"\n  ID:         {emp.Id}");
    Console.WriteLine($"  Name:       {emp.Name}");
    Console.WriteLine($"  Email:      {emp.Email}");
    Console.WriteLine($"  Job Role:   {emp.JobRole}");
    Console.WriteLine($"  System Role:{emp.SystemRole}");
    Console.WriteLine($"  Department: {(emp.HasDepartmentName ? emp.DepartmentName : "N/A")}\n");
}

static async Task GetAllDepartments(DepartmentGrpc.DepartmentGrpcClient client)
{
    var response = await client.GetAllAsync(new DepartmentEmpty());

    Console.WriteLine($"\n  Found {response.Departments.Count} department(s):");
    foreach (var dept in response.Departments)
    {
        Console.WriteLine($"    [{dept.DepartmentId}] {dept.DepartmentName} | Employees: {dept.EmployeeCount} | Manager: {(dept.HasManagerName ? dept.ManagerName : "N/A")}");
    }
    Console.WriteLine();
}

static async Task GetDepartmentById(DepartmentGrpc.DepartmentGrpcClient client)
{
    Console.Write("  Enter Department ID: ");
    if (!int.TryParse(Console.ReadLine(), out var id))
    {
        Console.WriteLine("  Invalid ID.\n");
        return;
    }

    var dept = await client.GetByIdAsync(new GetDepartmentRequest { Id = id });
    Console.WriteLine($"\n  ID:          {dept.DepartmentId}");
    Console.WriteLine($"  Name:        {dept.DepartmentName}");
    Console.WriteLine($"  Description: {(dept.HasDescription ? dept.Description : "N/A")}");
    Console.WriteLine($"  Manager:     {(dept.HasManagerName ? dept.ManagerName : "N/A")}");
    Console.WriteLine($"  Employees:   {dept.EmployeeCount}\n");
}

static async Task CreateEmployee(EmployeeGrpc.EmployeeGrpcClient client)
{
    Console.Write("  Name: ");
    var name = Console.ReadLine() ?? "";
    Console.Write("  Email: ");
    var email = Console.ReadLine() ?? "";
    Console.Write("  Job Role: ");
    var jobRole = Console.ReadLine() ?? "";
    Console.Write("  System Role: ");
    var systemRole = Console.ReadLine() ?? "";
    Console.Write("  Department ID (optional): ");
    var deptIdStr = Console.ReadLine();
    int deptId = 0;
    int.TryParse(deptIdStr, out deptId);
    var request = new CreateEmployeeRequest
    {
        Name = name,
        Email = email,
        JobRole = jobRole,
        SystemRole = systemRole,
        DepartmentId = deptId
    };
    var response = await client.CreateEmployeeAsync(request);
    Console.WriteLine($"\n  Created Employee: [{response.Id}] {response.Name} | {response.Email}\n");
}

static async Task UpdateEmployee(EmployeeGrpc.EmployeeGrpcClient client)
{
    Console.Write("  Employee ID: ");
    var idStr = Console.ReadLine();
    if (!int.TryParse(idStr, out var id))
    {
        Console.WriteLine("  Invalid ID.\n");
        return;
    }
    Console.Write("  Name: ");
    var name = Console.ReadLine() ?? "";
    Console.Write("  Email: ");
    var email = Console.ReadLine() ?? "";
    Console.Write("  Job Role: ");
    var jobRole = Console.ReadLine() ?? "";
    Console.Write("  System Role: ");
    var systemRole = Console.ReadLine() ?? "";
    Console.Write("  Department ID (optional): ");
    var deptIdStr = Console.ReadLine();
    int deptId = 0;
    int.TryParse(deptIdStr, out deptId);
    var request = new UpdateEmployeeRequest
    {
        Id = id,
        Name = name,
        Email = email,
        JobRole = jobRole,
        SystemRole = systemRole,
        DepartmentId = deptId
    };
    var response = await client.UpdateEmployeeAsync(request);
    Console.WriteLine($"\n  Updated Employee: [{response.Id}] {response.Name} | {response.Email}\n");
}

static async Task DeleteEmployee(EmployeeGrpc.EmployeeGrpcClient client)
{
    Console.Write("  Employee ID: ");
    var idStr = Console.ReadLine();
    if (!int.TryParse(idStr, out var id))
    {
        Console.WriteLine("  Invalid ID.\n");
        return;
    }
    var request = new DeleteEmployeeRequest { Id = id };
    var response = await client.DeleteEmployeeAsync(request);
    Console.WriteLine($"\n  Success: {response.Success}\n  Message: {response.Message}\n");
}







static async Task CreateDepartment(DepartmentGrpc.DepartmentGrpcClient client)
{
    Console.Write("  Department Name: ");
    var name = Console.ReadLine() ?? "";
    Console.Write("  Description (optional): ");
    var desc = Console.ReadLine() ?? "";
    Console.Write("  Manager ID (optional): ");
    var mgrIdStr = Console.ReadLine();
    int mgrId = 0;
    int.TryParse(mgrIdStr, out mgrId);
    var request = new CreateDepartmentRequest
    {
        DepartmentName = name,
        Description = desc,
        ManagerId = mgrId
    };
    var response = await client.CreateDepartmentAsync(request);
    Console.WriteLine($"\n  Created Department: [{response.DepartmentId}] {response.DepartmentName}\n");
}

static async Task UpdateDepartment(DepartmentGrpc.DepartmentGrpcClient client)
{
    Console.Write("  Department ID: ");
    var idStr = Console.ReadLine();
    if (!int.TryParse(idStr, out var id))
    {
        Console.WriteLine("  Invalid ID.\n");
        return;
    }
    Console.Write("  Department Name: ");
    var name = Console.ReadLine() ?? "";
    Console.Write("  Description (optional): ");
    var desc = Console.ReadLine() ?? "";
    Console.Write("  Manager ID (optional): ");
    var mgrIdStr = Console.ReadLine();
    int mgrId = 0;
    int.TryParse(mgrIdStr, out mgrId);
    var request = new UpdateDepartmentRequest
    {
        DepartmentId = id,
        DepartmentName = name,
        Description = desc,
        ManagerId = mgrId
    };
    var response = await client.UpdateDepartmentAsync(request);
    Console.WriteLine($"\n  Updated Department: [{response.DepartmentId}] {response.DepartmentName}\n");
}

static async Task DeleteDepartment(DepartmentGrpc.DepartmentGrpcClient client)
{
    Console.Write("  Department ID: ");
    var idStr = Console.ReadLine();
    if (!int.TryParse(idStr, out var id))
    {
        Console.WriteLine("  Invalid ID.\n");
        return;
    }
    var request = new DeleteDepartmentRequest { DepartmentId = id };
    var response = await client.DeleteDepartmentAsync(request);
    Console.WriteLine($"\n  Success: {response.Success}\n  Message: {response.Message}\n");
}

static async Task SendEmail(NotificationGrpc.NotificationGrpcClient client)
{
    Console.Write("  To Email: ");
    var to = Console.ReadLine() ?? "";
    Console.Write("  Subject:  ");
    var subject = Console.ReadLine() ?? "";
    Console.Write("  Body:     ");
    var body = Console.ReadLine() ?? "";

    var result = await client.SendEmailAsync(new SendEmailRequest
    {
        ToEmail = to,
        Subject = subject,
        Body = body
    });

    Console.WriteLine($"\n  Success: {result.Success}");
    Console.WriteLine($"  Message: {result.Message}\n");
}
