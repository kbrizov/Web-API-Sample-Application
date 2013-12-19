using ForumDb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumDb.Repositories
{
    public class DbPostRepository : IRepository<Post>
    {
        private readonly DbContext dbContext;
        private readonly DbSet<Post> postEntities;

        public DbPostRepository(DbContext dbContext)
        {
            this.dbContext = dbContext;
            this.postEntities = this.dbContext.Set<Post>();
        }

        public Post Add(Post entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("The entity being added cannot be null.");
            }

            this.postEntities.Add(entity);
            this.dbContext.SaveChanges();

            return entity;
        }

        public Post GetById(int id)
        {
            return this.postEntities.Find(id);
        }

        public IQueryable<Post> GetAll()
        {
            return this.postEntities;
        }

        public Post Update(int id, Post entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("The entity being used to upgrade cannot be null.");
            }

            Post toBeUpdated = this.postEntities.Find(id);
            if (toBeUpdated != null)
            {
                toBeUpdated.User = entity.User;
                toBeUpdated.Votes = entity.Votes;
                toBeUpdated.Thread = entity.Thread;
                toBeUpdated.DateCreated = entity.DateCreated;
                toBeUpdated.Content = entity.Content;
                toBeUpdated.Comments = entity.Comments;

                this.dbContext.SaveChanges();
            }

            return toBeUpdated;
        }

        public void Delete(int id)
        {
            var toBeDeleted = this.postEntities.Find(id);
            if (toBeDeleted != null)
            {
                this.postEntities.Remove(toBeDeleted);
                this.dbContext.SaveChanges();
            }
        }
    }
}
