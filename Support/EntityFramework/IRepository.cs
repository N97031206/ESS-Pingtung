using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace Support.EntityFramework
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        void Create(TEntity entity);

        TEntity ReadBy(Expression<Func<TEntity, bool>> predicate);

        IQueryable<TEntity> ReadListBy(Expression<Func<TEntity, bool>> predicate);

        IQueryable<TEntity> ReadAll();

        void Update(TEntity entity);

        void Delete(TEntity entity);

        void SaveChanges();

    }
}
