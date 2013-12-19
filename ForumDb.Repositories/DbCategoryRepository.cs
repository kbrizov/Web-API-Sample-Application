using System;
using System.Linq;
using ForumDb.Models;
using System.Data.Entity;

namespace ForumDb.Repositories
{
    public class DbCategoryRepository : IRepository<Category>
    {
        private readonly DbContext dbContext;
        private readonly DbSet<Category> categoryEnitities;

        public DbCategoryRepository(DbContext dbContext)
        {
            this.dbContext = dbContext;
            this.categoryEnitities = this.dbContext.Set<Category>();
        }

        public Category Add(Category entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("The entity being added cannot be null.");
            }

            this.categoryEnitities.Add(entity);
            this.dbContext.SaveChanges();

            return entity;
        }

        public Category GetById(int id)
        {
            return this.categoryEnitities.Find(id);
        }

        public IQueryable<Category> GetAll()
        {
            return this.categoryEnitities;
        }

        public Category Update(int id, Category entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("The entity being used to upgrade cannot be null.");
            }

            var entityToBeUpdated = this.categoryEnitities.Find(id);
            if (entityToBeUpdated != null)
            {
                entityToBeUpdated.Name = entity.Name;
                entityToBeUpdated.Threads = entity.Threads;

                this.dbContext.SaveChanges();
            }

            return entityToBeUpdated;
        }

        public void Delete(int id)
        {
            var category = this.categoryEnitities.Find(id);
            if (category != null)
            {
                this.categoryEnitities.Remove(category);
                this.dbContext.SaveChanges();
            }
        }
    }
}
