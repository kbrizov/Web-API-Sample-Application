using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace ForumDb.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : class
    {
        private IDbSet<T> dbSet;
        private DbContext context;

        public EfRepository(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentException("An instance of DbContext is required to use this repository.", "context");
            }

            this.context = context;
            this.dbSet = this.context.Set<T>();
        }

        public T Add(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("The entity being added cannot be null.");
            }

            DbEntityEntry entry = this.context.Entry(entity);
            if (entry.State != EntityState.Detached)
            {
                entry.State = EntityState.Added;
            }
            else
            {
                this.dbSet.Add(entity);
            }

            this.context.SaveChanges();
            return entity;
        }

        public T GetById(int id)
        {
            return this.dbSet.Find(id);
        }

        public IQueryable<T> GetAll()
        {
            return this.dbSet;
        }

        public T Update(int id, T entity)
        {
            DbEntityEntry entry = this.context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                this.dbSet.Attach(entity);
            }

            entry.State = EntityState.Modified;
            this.context.SaveChanges();

            return entity;
        }

        public void Delete(int id)
        {
            var entity = this.GetById(id);

            if (entity != null)
            {
                this.Delete(entity);
            }
        }

        public virtual void Delete(T entity)
        {
            DbEntityEntry entry = this.context.Entry(entity);
            if (entry.State != EntityState.Deleted)
            {
                entry.State = EntityState.Deleted;
            }
            else
            {
                this.dbSet.Attach(entity);
                this.dbSet.Remove(entity);
            }

            this.context.SaveChanges();
        }
    }
}
