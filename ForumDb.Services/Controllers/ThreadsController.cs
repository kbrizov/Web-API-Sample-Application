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
    public class ThreadsController : BaseApiController
    {
        private IRepository<Category> categoryRepository;
        private IRepository<Thread> threadRepository;
        private IRepository<User> userRepository;

        public ThreadsController(
            IRepository<Category> categoryRepository,
            IRepository<Thread> threadRepository,
            IRepository<User> userRepository)
        {
            this.categoryRepository = categoryRepository;
            this.threadRepository = threadRepository;
            this.userRepository = userRepository;
        }

        [HttpPost]
        [ActionName("create")]
        public HttpResponseMessage CreateThread(ThreadModel threadModel, string sessionKey)
        {
            var response = this.PerformOperationAndHandleExceptions(() =>
            {
                var user = this.userRepository.GetAll()
                    .Where(usr => usr.SessionKey == sessionKey).FirstOrDefault();

                if (user == null)
                {
                    throw new InvalidOperationException("User is not logged in or invalid session key.");
                }

                if (threadModel.Title == null)
                {
                    throw new ArgumentException("The title can't be null.");
                }

                if (threadModel.Content == null)
                {
                    throw new ArgumentException("The content can't be null.");
                }

                var newThread = new Thread()
                {
                    Title = threadModel.Title,
                    Content = threadModel.Content,
                    Creator = user,
                    DateCreated = threadModel.DateCreated,
                };

                foreach (var category in threadModel.Categories)
                {
                    var categoryEntity = this.categoryRepository.GetAll()
                        .Where(cat => cat.Name.ToLower() == category.ToLower()).FirstOrDefault();

                    if (categoryEntity == null)
                    {
                        categoryEntity = new Category();
                        categoryEntity.Name = category;
                        this.categoryRepository.Add(categoryEntity);
                    }

                    categoryEntity.Threads.Add(newThread);
                    newThread.Categories.Add(categoryEntity);
                }

                this.threadRepository.Add(newThread);

                var responseMessage = this.Request.CreateResponse(HttpStatusCode.Created,
                    new
                    {
                        id = newThread.Id,
                        title = newThread.Title
                    });

                return responseMessage;
            });

            return response;
        }

        [HttpGet]
        public IQueryable<ThreadModel> GetAll(string sessionKey)
        {
            var user = this.userRepository.GetAll().Where(
                usr => usr.SessionKey == sessionKey).FirstOrDefault();

            if (user == null)
            {
                throw new InvalidOperationException("No logged user with the given sessionKey.");
            }

            var userThreadEntities = user.Threads;
            var threadModels = new List<ThreadModel>();
            foreach (var threadEntity in userThreadEntities)
            {
                threadModels.Add(ThreadModel.CreateFromThreadEntity(threadEntity));
            }

            return threadModels.OrderByDescending(model => model.DateCreated).AsQueryable<ThreadModel>();
        }

        [HttpGet]
        public IQueryable<ThreadModel> GetByCategory([FromUri]string category, string sessionKey)
        {
            var threadModels = this.GetAll(sessionKey)
                .Where(thr => thr.Categories.Any(cat => cat.ToLower() == category.ToLower()));

            return threadModels;
        }

        [HttpGet]
        public IQueryable<ThreadModel> GetPage([FromUri]int page, [FromUri]int count, string sessionKey)
        {
            var threadModels = this.GetAll(sessionKey)
                .Skip(page * count)
                .Take(count);

            return threadModels;
        }

        [HttpGet]
        [ActionName("posts")]
        public IQueryable<PostModel> GetPosts([FromUri]int threadId, string sessionKey)
        {
            var user = this.userRepository.GetAll()
                .Where(usr => usr.SessionKey == sessionKey).FirstOrDefault();

            if (user == null)
            {
                throw new InvalidOperationException("No logged users with this session key or invalid session key.");
            }

            var threadEntity = this.threadRepository.GetAll()
                .Where(thr => thr.Id == threadId).FirstOrDefault();

            if (threadEntity != null)
            {
                var postEntities = threadEntity.Posts;
                var postModels = new List<PostModel>();

                foreach (var entity in postEntities)
                {
                    postModels.Add(PostModel.CreateFromPostEntity(entity));
                }

                return postModels.OrderByDescending(model => model.PostDate).AsQueryable();
            }

            return null;
        }
    }
}
