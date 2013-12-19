using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dependencies;
using ForumDb.Models;
using ForumDb.Repositories;
using ForumDb.Services.Controllers;
using ForumDb.Data;
using ForumDb.Services.Tests.FakeRepositories;

namespace ForumDb.IntegrationTests
{
    class TestDependencyResolver : IDependencyResolver
    {
        // TODO Fix it (write it all over again).

        //private IRepository<T> repository;

        //public IRepository<T> Repository
        //{
        //    get
        //    {
        //        return this.repository;
        //    }
        //    set
        //    {
        //        this.repository = value;
        //    }
        //}

        public IDependencyScope BeginScope()
        {
            return this;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(UsersController))
            {
                var repository = new FakeRepository<User>();
                var userController = new UsersController(repository);

                return userController;
            }
            else if (serviceType == typeof(ThreadsController))
            {
                var categoryRepository = new FakeRepository<Category>();
                var threadRepository = new FakeRepository<Thread>();
                var userRepository = new FakeRepository<User>();

                var threadController = new ThreadsController(
                    categoryRepository, threadRepository, userRepository);

                return threadController;
            }
            else if (serviceType == typeof(PostsController))
            {
                var postRepository = new FakeRepository<Post>();
                var userRepository = new FakeRepository<User>();
                var threadRepository = new FakeRepository<Thread>();
                var voteRepository = new FakeRepository<Vote>();
                var commentRepository = new FakeRepository<Comment>();

                var postsController = new PostsController(
                    postRepository, userRepository, threadRepository, voteRepository, commentRepository);

                return postsController;
            }
            else if (serviceType == typeof(CategoriesController))
            {
                var categoryRepository = new FakeRepository<Category>();
                var userRepository = new FakeRepository<User>();
                var threadRepository = new FakeRepository<Thread>();

                var categoriesController = new CategoriesController(categoryRepository, userRepository, threadRepository);

                return categoriesController;
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return new List<object>();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
