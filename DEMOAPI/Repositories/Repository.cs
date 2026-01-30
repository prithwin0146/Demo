using EmployeeApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EmployeeApi.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly TaskDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(TaskDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public (IEnumerable<T> Data, int TotalCount) GetPaged(int pageNumber, int pageSize)
        {
            var totalCount = _dbSet.Count();
            var data = _dbSet
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (data, totalCount);
        }

        public T? GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public T? Find(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }

        public bool Any(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Any(predicate);
        }

        public T Add(T entity)
        {
            _dbSet.Add(entity);
            return entity;
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
