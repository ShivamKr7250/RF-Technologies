using System.Linq.Expressions;

namespace RF_Technologies.Data_Access.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, bool tracked = false);
        T Get(Expression<Func<T, bool>>? filter, string? includeProperties = null, bool tracked = false);
        void Add(T entity);
        void Remove(T entity);
        bool Any(Expression<Func<T, bool>> filter);
    }
}
