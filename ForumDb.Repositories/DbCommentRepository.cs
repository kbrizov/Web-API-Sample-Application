using ForumDb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumDb.Repositories
{
    public class DbCommentRepository : IRepository<Comment>
    {
        private readonly DbContext dbContext;
        private readonly DbSet<Comment> commentEntities;

        public DbCommentRepository(DbContext dbContext)
        {
            this.dbContext = dbContext;
            this.commentEntities = this.dbContext.Set<Comment>();
        }

        public Comment Add(Comment entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("The entity being added cannot be null.");
            }

            this.commentEntities.Add(entity);
            this.dbContext.SaveChanges();

            return entity;
        }

        public Comment GetById(int id)
        {
            return this.commentEntities.Find(id);
        }

        public IQueryable<Comment> GetAll()
        {
            return this.commentEntities;
        }

        public Comment Update(int id, Comment entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("The entity being used to upgrade cannot be null.");
            }

            var toBeUpdated = this.commentEntities.Find(id);
            if (toBeUpdated != null)
            {
                toBeUpdated.Content = entity.Content;
                toBeUpdated.DateCreated = entity.DateCreated;
                toBeUpdated.Post = entity.Post;
                toBeUpdated.User = entity.User;

                this.dbContext.SaveChanges();
            }

            return toBeUpdated;
        }

        public void Delete(int id)
        {
            var toBeDeleted = this.commentEntities.Find(id);
            if (toBeDeleted != null)
            {
                this.commentEntities.Remove(toBeDeleted);
                this.dbContext.SaveChanges();
            }
        }
    }
}
