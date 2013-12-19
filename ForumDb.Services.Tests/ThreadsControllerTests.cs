using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ForumDb.Services.Tests.FakeRepositories;
using ForumDb.Models;
using ForumDb.Services.Models;
using ForumDb.Services.Controllers;

namespace ForumDb.Services.Tests
{
    [TestClass]
    public class ThreadsControllerTests
    {
        private List<Route> GetRoutes()
        {
            List<Route> routeList = new List<Route>();

            Route defaultApiRoute = new Route(
                "DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });

            Route ThreadsPostsApiRoute = new Route(
                "ThreadsPostsApi", "api/threads/{threadId}/{action}", new { controller = "threads", action = "posts" });

            routeList.Add(defaultApiRoute);
            routeList.Add(ThreadsPostsApiRoute);

            return routeList;
        }

        private void AddHttpRoutesToRouteCollection(HttpRouteCollection routeCollection)
        {
            var routes = this.GetRoutes();
            foreach (var route in routes)
            {
                routeCollection.MapHttpRoute(route.Name, route.Template, route.Defaults);
            }
        }

        private void SetupController(ApiController controller)
        {
            var config = new HttpConfiguration();
            this.AddHttpRoutesToRouteCollection(config.Routes);

            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/threads");
            request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, config);
            
            controller.Configuration = config;
            controller.Request = request;
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Post_CreateThreadTest_InvalidCreation_SessionKeyIsNull()
        {
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();
            FakeRepository<Category> categoryFakeRepository = new FakeRepository<Category>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();

            ThreadsController threadsController = new ThreadsController(
                categoryFakeRepository, threadFakeRepository, userFakeRepository);

            SetupController(threadsController);

            ThreadModel threadModel = new ThreadModel()
            {
                Title = "Test title",
                Content = "Test content",
                CreatedBy = "Test User",
                DateCreated = DateTime.Now
            };

            string userSessionKey = null;
            threadsController.CreateThread(threadModel, userSessionKey);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Post_CreateThreadTest_InvalidCreation_ThreadTitleIsNull()
        {
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();
            FakeRepository<Category> categoryFakeRepository = new FakeRepository<Category>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();

            ThreadsController threadsController = new ThreadsController(
                categoryFakeRepository, threadFakeRepository, userFakeRepository);

            SetupController(threadsController);

            User user = new User()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "TestUsername",
                Nickname = "TestNickname",
                SessionKey = "0SuGqVGqRwitYtijDvHlVfHGotklitbwHdYFkgwIRcIQjRASPQ"
            };

            userFakeRepository.entities.Add(user);

            ThreadModel threadModel = new ThreadModel()
            {
                Title = null,
                Content = "Test content",
                CreatedBy = "Test User",
                DateCreated = DateTime.Now
            };

            threadsController.CreateThread(threadModel, user.SessionKey);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Post_CreateThreadTest_InvalidCreation_ThreadContentIsNull()
        {
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();
            FakeRepository<Category> categoryFakeRepository = new FakeRepository<Category>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();

            ThreadsController threadsController = new ThreadsController(
                categoryFakeRepository, threadFakeRepository, userFakeRepository);

            SetupController(threadsController);

            User user = new User()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "TestUsername",
                Nickname = "TestNickname",
                SessionKey = "0SuGqVGqRwitYtijDvHlVfHGotklitbwHdYFkgwIRcIQjRASPQ"
            };

            userFakeRepository.entities.Add(user);

            ThreadModel threadModel = new ThreadModel()
            {
                Title = "Test title",
                Content = null,
                CreatedBy = "Test User",
                DateCreated = DateTime.Now
            };

            threadsController.CreateThread(threadModel, user.SessionKey);
        }

        [TestMethod]
        public void Post_CreateThreadTest_ValidCreation()
        {
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();
            FakeRepository<Category> categoryFakeRepository = new FakeRepository<Category>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();

            User user = new User()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "TestUsername",
                Nickname = "TestNickname",
                SessionKey = "0SuGqVGqRwitYtijDvHlVfHGotklitbwHdYFkgwIRcIQjRASPQ"
            };

            userFakeRepository.entities.Add(user);

            Category category1 = new Category()
            {
                Name = "Test category 1"
            };

            categoryFakeRepository.entities.Add(category1);

            ThreadsController threadsController = new ThreadsController(
                categoryFakeRepository, threadFakeRepository, userFakeRepository);

            SetupController(threadsController);

            ThreadModel threadModel = new ThreadModel()
            {
                Title = "Test title",
                Content = "Test content",
                CreatedBy = "Test User",
                DateCreated = DateTime.Now,
                Categories = new List<string>()
                {
                    "Test category 1",
                    "Test category 2"
                }
            };

            threadsController.CreateThread(threadModel, user.SessionKey);
            
            int expectedThreadCount = 1;
            Thread addedThread = threadFakeRepository.entities.FirstOrDefault();
            List<Category> addedThreadCategories = addedThread.Categories.ToList<Category>();

            Assert.AreEqual(expectedThreadCount, threadFakeRepository.entities.Count);
            Assert.AreEqual("Test category 1", addedThreadCategories[0].Name);
            Assert.AreEqual("Test category 2", addedThreadCategories[1].Name);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Get_GetAllTest_InvalidSessionKey_NoUserWithSuchSessionKey()
        {
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();
            FakeRepository<Category> categoryFakeRepository = new FakeRepository<Category>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();

            ThreadsController threadsController = new ThreadsController(
                categoryFakeRepository, threadFakeRepository, userFakeRepository);

            SetupController(threadsController);

            User user = new User()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "TestUsername",
                Nickname = "TestNickname",
                SessionKey = "0SuGqVGqRwitYtijDvHlVfHGotklitbwHdYFkgwIRcIQjRASPQ"
            };               

            userFakeRepository.entities.Add(user);

            threadsController.GetAll("1InvalidSessionKeyvHlVfHGotklitbwHdYFkgwIRcIQjRAPQ");
        }

        [TestMethod]
        public void Get_GetAllTest_ValidExecutionOfTheMethod()
        {
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();
            FakeRepository<Category> categoryFakeRepository = new FakeRepository<Category>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();

            User user = new User()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "TestUsername",
                Nickname = "TestNickname",
                SessionKey = "0SuGqVGqRwitYtijDvHlVfHGotklitbwHdYFkgwIRcIQjRASPQ"
            };

            Thread thread = new Thread()
            {
                Title = "Test title",
                Creator = user,
                Content = "Test Content",
                DateCreated = DateTime.Now
            };

            user.Threads.Add(thread);
            userFakeRepository.entities.Add(user);

            ThreadsController threadsController = new ThreadsController(
                categoryFakeRepository, threadFakeRepository, userFakeRepository);

            SetupController(threadsController);

            var allThreads = threadsController.GetAll("0SuGqVGqRwitYtijDvHlVfHGotklitbwHdYFkgwIRcIQjRASPQ").ToList<ThreadModel>();
            int expectedThreadCount = 1;

            Assert.AreEqual(expectedThreadCount, allThreads.Count);
        }

        [TestMethod]
        public void Get_GetAllTest_CheckForCorrectDescendingOrder()
        {
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();
            FakeRepository<Category> categoryFakeRepository = new FakeRepository<Category>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();

            User user = new User()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "TestUsername",
                Nickname = "TestNickname",
                SessionKey = "0SuGqVGqRwitYtijDvHlVfHGotklitbwHdYFkgwIRcIQjRASPQ"
            };

            for (int i = 1; i <= 5; i++)
            {
                Thread thread = new Thread()
                {
                    Title = string.Format("Test title {0}", i),
                    Creator = user,
                    Content = string.Format("Test content {0}", i),
                    DateCreated = DateTime.Now.AddDays(i)
                };

                user.Threads.Add(thread);
            }

            userFakeRepository.entities.Add(user);

            ThreadsController threadsController = new ThreadsController(
                categoryFakeRepository, threadFakeRepository, userFakeRepository);

            SetupController(threadsController);

            var allThreads = threadsController.GetAll("0SuGqVGqRwitYtijDvHlVfHGotklitbwHdYFkgwIRcIQjRASPQ").ToArray<ThreadModel>();

            for (int i = 0; i < allThreads.Length - 1; i++)
            {
                int result = allThreads[i].DateCreated.CompareTo(allThreads[i + 1].DateCreated);

                Assert.IsTrue(result > 0);
            }
        }

        [TestMethod]
        public void Get_GetByCategoryTest_NoSuchThreadInThisCategory_ShouldReturnEmptyIQueryable()
        {
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();
            FakeRepository<Category> categoryFakeRepository = new FakeRepository<Category>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            string testCategory = "Test category";

            User user = new User()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "TestUsername",
                Nickname = "TestNickname",
                SessionKey = "0SuGqVGqRwitYtijDvHlVfHGotklitbwHdYFkgwIRcIQjRASPQ"
            };

            Category category = new Category()
            {
                Name = testCategory
            };

            Thread thread = new Thread()
            {
                Title = "Test title",
                Content = "Test content",
                DateCreated = DateTime.Now,
                Creator = user
            };

            thread.Categories.Add(category);
            category.Threads.Add(thread);
            //user.Threads.Add(thread);

            userFakeRepository.entities.Add(user);
            threadFakeRepository.Add(thread);
            categoryFakeRepository.Add(category);

            ThreadsController threadsController = new ThreadsController(
                categoryFakeRepository, threadFakeRepository, userFakeRepository);

            SetupController(threadsController);

            var threads = threadsController.GetByCategory(testCategory, user.SessionKey).ToList<ThreadModel>();
            int expectedThreadCount = 0;

            Assert.AreEqual(expectedThreadCount, threads.Count);
        }

        [TestMethod]
        public void Get_GetByCategoryTest_OnlyOneThreadWithThatCategory_ShouldBeReturned()
        {
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();
            FakeRepository<Category> categoryFakeRepository = new FakeRepository<Category>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            string testCategory = "Test category";

            User user = new User()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "TestUsername",
                Nickname = "TestNickname",
                SessionKey = "0SuGqVGqRwitYtijDvHlVfHGotklitbwHdYFkgwIRcIQjRASPQ"
            };

            Category category = new Category()
            {
                Name = testCategory
            };

            Thread thread = new Thread()
            {
                Title = "Test title",
                Content = "Test content",
                DateCreated = DateTime.Now,
                Creator = user
            };

            thread.Categories.Add(category);
            category.Threads.Add(thread);
            user.Threads.Add(thread);

            userFakeRepository.entities.Add(user);
            threadFakeRepository.Add(thread);
            categoryFakeRepository.Add(category);

            ThreadsController threadsController = new ThreadsController(
                categoryFakeRepository, threadFakeRepository, userFakeRepository);

            SetupController(threadsController);

            var threads = threadsController.GetByCategory(testCategory, user.SessionKey).ToList<ThreadModel>();
            int expectedThreadCount = 1;

            Assert.AreEqual(expectedThreadCount, threads.Count);
        }

        [TestMethod]
        public void Get_GetPageTest_GettingLast5ThreadsFromTotalOf30()
        {
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();
            FakeRepository<Category> categoryFakeRepository = new FakeRepository<Category>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();

            User user = new User()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "TestUsername",
                Nickname = "TestNickname",
                SessionKey = "0SuGqVGqRwitYtijDvHlVfHGotklitbwHdYFkgwIRcIQjRASPQ"
            };

            for (int i = 1; i <= 30; i++)
            {
                Thread thread = new Thread()
                {
                    Title = string.Format("Test title {0}", i),
                    Content = string.Format("Test content {0}", i),
                    DateCreated = DateTime.Now.AddDays(i),
                    Creator = user
                };

                threadFakeRepository.entities.Add(thread);
                user.Threads.Add(thread);
            }

            userFakeRepository.entities.Add(user);

            ThreadsController threadsController = new ThreadsController(
                categoryFakeRepository, threadFakeRepository, userFakeRepository);

            SetupController(threadsController);

            var requestedThreads = threadsController.GetPage(5, 5, user.SessionKey).ToArray<ThreadModel>();
            int expectedThreadCount = 5;

            Assert.AreEqual(expectedThreadCount, requestedThreads.Length);

            // Checking the last five threads.
            for (int i = 0; i < requestedThreads.Length; i++)
            {
                string expectedTitle = string.Format("Test title {0}", requestedThreads.Length - i);
                string expectedContent = string.Format("Test content {0}", requestedThreads.Length - i);

                Assert.AreEqual(expectedTitle, requestedThreads[i].Title);
                Assert.AreEqual(expectedContent, requestedThreads[i].Content);
            }
        }

        [TestMethod]
        public void Get_GetPostsTest_GetsThePostsWithGivenThreadId_InvalidThreadId_ShouldReturnNull()
        {
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();
            FakeRepository<Category> categoryFakeRepository = new FakeRepository<Category>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();

            User user = new User()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "TestUsername",
                Nickname = "TestNickname",
                SessionKey = "0SuGqVGqRwitYtijDvHlVfHGotklitbwHdYFkgwIRcIQjRASPQ"
            };

            Thread thread = new Thread()
            {
                Title = "Test title",
                Creator = user,
                Content = "Test Content",
                DateCreated = DateTime.Now
            };

            for (int i = 1; i <= 20; i++)
            {
                Post post = new Post()
                {
                    Content = "Test post content",
                    DateCreated = DateTime.Now.AddDays(i)
                };

                thread.Posts.Add(post);
            }

            user.Threads.Add(thread);
            userFakeRepository.entities.Add(user);
            threadFakeRepository.entities.Add(thread);

            ThreadsController threadsController = new ThreadsController(
                categoryFakeRepository, threadFakeRepository, userFakeRepository);

            SetupController(threadsController);

            int unexistingThreadId = 100;
            var posts = threadsController.GetPosts(unexistingThreadId, user.SessionKey);

            Assert.IsNull(posts);
        }

        [TestMethod]
        public void Get_GetPostsTest_GetsThePostsWithGivenThreadId_ShouldReturnThemAll()
        {
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();
            FakeRepository<Category> categoryFakeRepository = new FakeRepository<Category>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();

            User user = new User()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "TestUsername",
                Nickname = "TestNickname",
                SessionKey = "0SuGqVGqRwitYtijDvHlVfHGotklitbwHdYFkgwIRcIQjRASPQ"
            };

            Thread thread = new Thread()
            {
                Title = "Test title",
                Creator = user,
                Content = "Test Content",
                DateCreated = DateTime.Now
            };

            for (int i = 1; i <= 20; i++)
            {
                Post post = new Post()
                {
                    Content = string.Format("Test post content {0}", i),
                    DateCreated = DateTime.Now.AddDays(i),
                    User = user
                };

                post.Thread = thread;
                thread.Posts.Add(post);
            }

            user.Threads.Add(thread);
            userFakeRepository.entities.Add(user);
            threadFakeRepository.entities.Add(thread);

            ThreadsController threadsController = new ThreadsController(
                categoryFakeRepository, threadFakeRepository, userFakeRepository);

            SetupController(threadsController);
            
            var posts = threadsController.GetPosts(thread.Id, user.SessionKey).ToList<PostModel>();

            int expectedCount = 20;
            Assert.AreEqual(expectedCount, posts.Count);
        }

        [TestMethod]
        public void Get_GetPostsTest_CheckForCorrectDescendingOrder()
        {
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();
            FakeRepository<Category> categoryFakeRepository = new FakeRepository<Category>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();

            User user = new User()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "TestUsername",
                Nickname = "TestNickname",
                SessionKey = "0SuGqVGqRwitYtijDvHlVfHGotklitbwHdYFkgwIRcIQjRASPQ"
            };

            Thread thread = new Thread()
            {
                Title = "Test title",
                Creator = user,
                Content = "Test Content",
                DateCreated = DateTime.Now
            };

            for (int i = 1; i <= 5; i++)
            {
                Post post = new Post()
                {
                    Content = string.Format("Test post content {0}", i),
                    DateCreated = DateTime.Now.AddDays(i),
                    User = user
                };

                post.Thread = thread;
                thread.Posts.Add(post);
            }

            user.Threads.Add(thread);
            userFakeRepository.entities.Add(user);
            threadFakeRepository.entities.Add(thread);

            ThreadsController threadsController = new ThreadsController(
                categoryFakeRepository, threadFakeRepository, userFakeRepository);

            SetupController(threadsController);

            var posts = threadsController.GetPosts(thread.Id, user.SessionKey).ToArray<PostModel>();

            for (int i = 0; i < posts.Length - 1; i++)
            {
                int result = posts[i].PostDate.CompareTo(posts[i + 1].PostDate);

                Assert.IsTrue(result > 0);
            }
        }
    }
}
