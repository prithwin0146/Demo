using System;
using System.Collections.Generic;

namespace EmployeeApi.Models;

public partial class Employee
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string JobRole { get; set; } = null!;

    public string Role { get; set; } = "Employee";

    public int? DepartmentId { get; set; }
}
