using ForumDb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumDb.Repositories
{
    public class DbThreadRepository : IRepository<Thread>
    {
        private readonly DbContext dbContext;
        private readonly DbSet<Thread> threadEntities;

        public DbThreadRepository(DbContext dbContext)
        {
            this.dbContext = dbContext;
            this.threadEntities = this.dbContext.Set<Thread>();
        }

        public Thread Add(Thread entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("The entity being added cannot be null.");
            }

            this.threadEntities.Add(entity);
            this.dbContext.SaveChanges();

            return entity;
        }

        public Thread GetById(int id)
        {
            return this.threadEntities.Find(id);
        }

        public IQueryable<Thread> GetAll()
        {
            return this.threadEntities;
        }

        public Thread Update(int id, Thread entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("The entity being used to upgrade cannot be null.");
            }

            Thread toBeUpdated = this.threadEntities.Find(id);
            if (toBeUpdated != null)
            {
                toBeUpdated.Categories = entity.Categories;
                toBeUpdated.Content = entity.Content;
                toBeUpdated.Creator = entity.Creator;
                toBeUpdated.DateCreated = entity.DateCreated;
                toBeUpdated.Posts = entity.Posts;
                toBeUpdated.Title = entity.Title;

                this.dbContext.SaveChanges();
            }

            return toBeUpdated;
        }

        public void Delete(int id)
        {
            var toBeDeleted = this.threadEntities.Find(id);
            if (toBeDeleted != null)
            {
                this.threadEntities.Remove(toBeDeleted);
                this.dbContext.SaveChanges();
            }
        }
    }
}
