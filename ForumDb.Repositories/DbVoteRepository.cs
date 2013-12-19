using ForumDb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumDb.Repositories
{
    public class DbVoteRepository : IRepository<Vote>
    {
        private readonly DbContext dbContext;
        private readonly DbSet<Vote> voteEntities;

        public DbVoteRepository(DbContext dbContext)
        {
            this.dbContext = dbContext;
            this.voteEntities = this.dbContext.Set<Vote>();
        }

        public Vote Add(Vote entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("The entity being added cannot be null.");
            }

            this.voteEntities.Add(entity);
            this.dbContext.SaveChanges();

            return entity;
        }

        public Vote GetById(int id)
        {
            return this.voteEntities.Find(id);
        }

        public IQueryable<Vote> GetAll()
        {
            return this.voteEntities;
        }

        public Vote Update(int id, Vote entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("The entity being used to upgrade cannot be null.");
            }

            var toBeUpdated = this.voteEntities.Find(id);
            if (toBeUpdated != null)
            {
                toBeUpdated.Post = entity.Post;
                toBeUpdated.User = entity.User;
                toBeUpdated.Value = entity.Value;

                this.dbContext.SaveChanges();
            }

            return toBeUpdated;
        }

        public void Delete(int id)
        {
            var toBeDeleted = this.voteEntities.Find(id);
            if (toBeDeleted != null)
            {
                this.voteEntities.Remove(toBeDeleted);
                this.dbContext.SaveChanges();
            }
        }
    }
}
