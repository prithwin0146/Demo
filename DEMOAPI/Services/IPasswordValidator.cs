namespace EmployeeApi.Services;

public interface IPasswordValidator
{
    (bool IsValid, string ErrorMessage) Validate(string password);
}

public class PasswordValidator : IPasswordValidator
{
    public (bool IsValid, string ErrorMessage) Validate(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return (false, "Password is required");

        if (password.Length < 8)
            return (false, "Password must be at least 8 characters long");

        return (true, string.Empty);
    }
}
