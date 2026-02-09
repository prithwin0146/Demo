using EmployeeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApi.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(TaskDbContext context) : base(context)
        {
        }

        public User? GetByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public bool ExistsWithEmail(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }

        public void UpdateRole(string email, string role)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                user.Role = role;
                _context.SaveChanges();
            }
        }
    }
}
