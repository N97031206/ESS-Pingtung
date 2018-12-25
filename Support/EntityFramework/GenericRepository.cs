using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;

namespace Support.EntityFramework
{
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private DbContext db { get; set; }

        public GenericRepository(DbContext _db)
        {
            if (_db == null)
            {
                throw new ArgumentNullException();
            }
            this.db = _db;
        }

        public virtual void Create(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            this.db.Entry(entity).State = EntityState.Added;
        }

        public virtual TEntity ReadBy(Expression<Func<TEntity, bool>> predicate)
        {
            return this.db.Set<TEntity>().FirstOrDefault(predicate);
        }


        public virtual IQueryable<TEntity> ReadListBy(Expression<Func<TEntity, bool>> predicate)
        {
            return this.db.Set<TEntity>().Where(predicate);
        }

        public virtual IQueryable<TEntity> ReadAll()
        {
            return this.db.Set<TEntity>().AsQueryable();
        }

        public virtual void Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            this.db.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }
            this.db.Entry(entity).State = EntityState.Deleted;
        }

        public virtual IQueryable<TEntity> Skip(int count)
        {
            return this.db.Set<TEntity>().Skip(count);
        }

        public virtual IQueryable<TEntity> Take(int count)
        {
            return this.db.Set<TEntity>().Take(count);
        }

        public virtual void SaveChanges()
        {
            try
            {
                this.db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                var entityError = ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage);
                var getFullMessage = string.Join(";", entityError);
                var exceptionMessage = string.Concat(ex.Message, "errors are:", getFullMessage);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.db != null)
                {
                    this.db.Dispose();
                    this.db = null;
                }
            }
        }
    }
}
