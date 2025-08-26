

using System.Linq.Expressions;

namespace Demo.DataAccess.Repositories.Classes
{
    public class GenericRepository<TEntity>(ApplicationDbContext _dbContext) : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        // Get All
        public IEnumerable<TEntity> GetAll(bool withTracking = false)
        {
            return withTracking == true ?
            _dbContext.Set<TEntity>().Where(E => E.IsDeleted != true).ToList() :
                     _dbContext.Set<TEntity>().Where(E => E.IsDeleted != true).AsNoTracking().ToList();
        }

        // Get By Id
        public TEntity? GetById(int id)
        {
            var Employee = _dbContext.Set<TEntity>().Find(id);
            return Employee;
        }
        // Update
        public void Update(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
        }

        // Delete
        public void Remove(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
        }

        // Insert
        public void  Add(TEntity entity)
        {
            _dbContext.Set<TEntity>().Add(entity);
        }

        public IEnumerable<TResult> GetAll<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            return _dbContext.Set<TEntity>().Where(E => E.IsDeleted != true)
                             .Select(selector).ToList();
        }

        public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> Predicate)
        {
            return _dbContext.Set<TEntity>()
                            .Where(Predicate)
                            .ToList();
        }
    }
}
