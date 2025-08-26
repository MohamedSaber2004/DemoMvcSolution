
using System.Linq.Expressions;

namespace Demo.DataAccess.Repositories.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity:BaseEntity
    {
        void Add(TEntity entity);
        IEnumerable<TEntity> GetAll(bool withTracking = false);
        IEnumerable<TResult> GetAll<TResult>(Expression<Func<TEntity,TResult>> selector);
        IEnumerable<TEntity> GetAll(Expression<Func<TEntity,bool>> Predicate);
        TEntity? GetById(int id);
        void Remove(TEntity TEntity);
        void Update(TEntity TEntity);
    }
}
