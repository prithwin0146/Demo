using EmployeeApi.Models;

namespace EmployeeApi.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        User? GetByEmail(string email);
    }
}
