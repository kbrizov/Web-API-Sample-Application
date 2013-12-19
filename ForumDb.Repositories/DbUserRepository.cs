using ForumDb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumDb.Repositories
{
    public class DbUserRepository : IRepository<User>
    {
        private readonly DbContext dbContext;
        private readonly DbSet<User> userEntities;

        public DbUserRepository(DbContext dbContext)
        {
            this.dbContext = dbContext;
            this.userEntities = this.dbContext.Set<User>();
        }

        public User Add(User entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("The entity being added cannot be null.");
            }

            this.userEntities.Add(entity);
            this.dbContext.SaveChanges();

            return entity;
        }

        public User GetById(int id)
        {
            return this.userEntities.Find(id);
        }

        public IQueryable<User> GetAll()
        {
            return this.userEntities;
        }

        public User Update(int id, User entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("The entity being used to upgrade cannot be null.");
            }

            var entityToBeUpdated = this.userEntities.Find(id);
            if (entityToBeUpdated != null)
            {
                entityToBeUpdated.Username = entity.Username;
                entityToBeUpdated.Nickname = entity.Nickname;
                entityToBeUpdated.Posts = entity.Posts;
                entityToBeUpdated.Votes = entity.Votes;
                entityToBeUpdated.Comments = entity.Comments;
                entityToBeUpdated.AuthCode = entity.AuthCode;
                entityToBeUpdated.SessionKey = entity.SessionKey;
                entityToBeUpdated.Threads = entity.Threads;

                this.dbContext.SaveChanges();
            }

            return entityToBeUpdated;
        }

        public void Delete(int id)
        {
            var toBeDeleted = this.userEntities.Find(id);
            if (toBeDeleted != null)
            {
                this.userEntities.Remove(toBeDeleted);
                this.dbContext.SaveChanges();
            }
        }
    }
}
