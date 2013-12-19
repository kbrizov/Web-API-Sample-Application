using ForumDb.Models;
using ForumDb.Repositories;
using ForumDb.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ForumDb.Services.Controllers
{
    public class CategoriesController : BaseApiController
    {
        private IRepository<Category> categoryRepository;
        private IRepository<User> userRepository;
        private IRepository<Thread> threadRepository;

        public CategoriesController(
            IRepository<Category> categoryRepository,
            IRepository<User> userRepository,
            IRepository<Thread> threadRepository)
        {
            this.categoryRepository = categoryRepository;
            this.userRepository = userRepository;
            this.threadRepository = threadRepository;
        }

        [HttpPost]
        [ActionName("create")]
        public HttpResponseMessage CreateCategory(CategoryModel model, string sessionKey)
        {
            var responseMessage = this.PerformOperationAndHandleExceptions(() =>
                {
                    var user = this.userRepository.GetAll()
                    .Where(usr => usr.SessionKey == sessionKey).FirstOrDefault();

                    if (user == null)
                    {
                        throw new InvalidOperationException("User is not logged in or invalid session key.");
                    }

                    if (model == null)
                    {
                        throw new ArgumentNullException("The CategoryModel cannot be null.");
                    }

                    if (model.Name == null)
                    {
                        throw new ArgumentNullException("The category name cannot be null.");
                    }

                    Category category = new Category();
                    category.Name = model.Name;

                    var threadTitles = model.threadTitles;
                    var allThreadEntities = this.threadRepository.GetAll();

                    if (threadTitles != null)
                    {
                        foreach (var title in threadTitles)
                        {
                            var threadEntity = allThreadEntities.FirstOrDefault(
                                entity => entity.Title == title);

                            category.Threads.Add(threadEntity);
                        }
                    }

                    this.categoryRepository.Add(category);

                    var response = this.Request.CreateResponse(HttpStatusCode.Created, new { id = category.Id });
                    return response;
                });

            return responseMessage;
        }

        [HttpGet]
        public IQueryable<string> GetAll(string sessionKey)
        {
            var user = this.userRepository.GetAll()
                .Where(usr => usr.SessionKey == sessionKey).FirstOrDefault();

            if (user == null)
            {
                throw new InvalidOperationException("No user logged or invalid session key.");
            }

            var categories = this.categoryRepository.GetAll().Select(cat => cat.Name);
            return categories;
        }
    }
}
