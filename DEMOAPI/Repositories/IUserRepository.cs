using EmployeeApi.Models;

namespace EmployeeApi.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        User? GetByEmail(string email);
        Task<bool> ExistsWithEmailAsync(string email);
    }
}
