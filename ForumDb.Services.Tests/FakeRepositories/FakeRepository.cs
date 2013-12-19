using ForumDb.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumDb.Services.Tests.FakeRepositories
{
    public class FakeRepository<T> : IRepository<T> where T : class
    {
        public IList<T> entities;

        public FakeRepository()
        {
            this.entities = new List<T>();
        }

        public T Add(T entity)
        {
            this.entities.Add(entity);

            return entity;
        }

        public T GetById(int id)
        {
            return this.entities[id];
        }

        public IQueryable<T> GetAll()
        {
            return this.entities.AsQueryable<T>();
        }

        public T Update(int id, T entity)
        {
            this.entities[id] = entity;

            return this.entities[id];
        }

        public void Delete(int id)
        {
            this.entities.RemoveAt(id);
        }
    }
}
