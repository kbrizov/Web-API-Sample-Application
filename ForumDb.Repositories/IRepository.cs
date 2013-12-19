using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumDb.Repositories
{
    public interface IRepository<T> where T : class
    {
        T Add(T entity);

        T GetById(int id);

        IQueryable<T> GetAll();

        T Update(int id, T entity);

        void Delete(int id);
    }
}
