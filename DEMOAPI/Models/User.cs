using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EmployeeApi.Models;

public partial class User
{
    public int Id { get; set; }

    public string? Username { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Role { get; set; }
}
