using EmployeeApi.Models;

namespace EmployeeApi.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        User? GetByEmail(string email);
        bool ExistsWithEmail(string email);
        void UpdateRole(string email, string role);
    }
}
