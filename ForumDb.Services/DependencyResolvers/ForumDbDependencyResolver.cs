using ForumDb.Data;
using ForumDb.Models;
using ForumDb.Repositories;
using ForumDb.Services.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Dependencies;

namespace ForumDb.Services.DependencyResolvers
{
    public class ForumDbDependencyResolver: IDependencyResolver
    {
        public IDependencyScope BeginScope()
        {
            return this;
        }

        public object GetService(Type serviceType)
        {
            var dbContext = new ForumDbContext();

            if (serviceType == typeof(UsersController))
            {
                var repository = new EfRepository<User>(dbContext);
                var userController = new UsersController(repository);

                return userController;
            }
            else if (serviceType == typeof(ThreadsController))
            {
                var categoryRepository = new EfRepository<Category>(dbContext);
                var threadRepository = new EfRepository<Thread>(dbContext);
                var userRepository = new EfRepository<User>(dbContext);

                var threadController = new ThreadsController(
                    categoryRepository, threadRepository, userRepository);

                return threadController;
            }
            else if (serviceType == typeof(PostsController))
            {
                var postRepository = new EfRepository<Post>(dbContext);
                var userRepository = new EfRepository<User>(dbContext);
                var threadRepository = new EfRepository<Thread>(dbContext);
                var voteRepository = new EfRepository<Vote>(dbContext);
                var commentRepository = new EfRepository<Comment>(dbContext);

                var postsController = new PostsController(
                    postRepository, userRepository, threadRepository, voteRepository, commentRepository);

                return postsController;
            }
            else if (serviceType == typeof(CategoriesController))
            {
                var categoryRepository = new EfRepository<Category>(dbContext);
                var userRepository = new EfRepository<User>(dbContext);
                var threadRepository = new EfRepository<Thread>(dbContext);

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