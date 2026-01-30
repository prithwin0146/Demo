using System.Linq.Expressions;

namespace EmployeeApi.Repositories
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        (IEnumerable<T> Data, int TotalCount) GetPaged(int pageNumber, int pageSize);
        T? GetById(int id);
        T? Find(Expression<Func<T, bool>> predicate);
        bool Any(Expression<Func<T, bool>> predicate);
        T Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<int> SaveChangesAsync();
    }
}
