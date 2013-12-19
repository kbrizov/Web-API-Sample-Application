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
    public class PostsContollerTests
    {
        private List<Route> GetRoutes()
        {
            List<Route> routeList = new List<Route>();

            Route defaultApiRoute = new Route(
                "DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });

            Route PostsApiRoute = new Route(
                "PostsApi", "api/posts/{postId}/{action}", new { controller = "posts" });

            Route PostsCreateApi = new Route(
                "PostCreateApi", "api/posts/{action}", new { controller = "posts", action = "create" });

            routeList.Add(defaultApiRoute);
            routeList.Add(PostsApiRoute);
            routeList.Add(PostsCreateApi);

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

            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/posts");
            request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, config);

            controller.Configuration = config;
            controller.Request = request;
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Get_GetAllTest_InvalidSessionKey_NoUserWithSuchSessionKey()
        {
            FakeRepository<Post> postFakeRepository = new FakeRepository<Post>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();
            FakeRepository<Vote> voteFakeRepository = new FakeRepository<Vote>();
            FakeRepository<Comment> commentFakeRepository = new FakeRepository<Comment>();

            User user = new User()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "TestUsername",
                Nickname = "TestNickname",
                SessionKey = "0SuGqVGqRwitYtijDvHlVfHGotklitbwHdYFkgwIRcIQjRASPQ"
            };

            userFakeRepository.entities.Add(user);

            PostsController threadsController = new PostsController(
                postFakeRepository, userFakeRepository, threadFakeRepository, voteFakeRepository, commentFakeRepository);

            SetupController(threadsController);

            threadsController.GetAll("1InvalidSessionKeyvHlVfHGotklitbwHdYFkgwIRcIQjRAPQ");
        }

        [TestMethod]
        public void Get_GetAllTest_ValidExecution_ReturningSomePosts()
        {
            FakeRepository<Post> postFakeRepository = new FakeRepository<Post>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();
            FakeRepository<Vote> voteFakeRepository = new FakeRepository<Vote>();
            FakeRepository<Comment> commentFakeRepository = new FakeRepository<Comment>();

            User user = new User()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "TestUsername",
                Nickname = "TestNickname",
                SessionKey = "0SuGqVGqRwitYtijDvHlVfHGotklitbwHdYFkgwIRcIQjRASPQ"
            };

            for (int i = 1; i <= 20; i++)
            {
                Post post = new Post()
                {
                    Content = string.Format("Test post content {0}", i),
                    DateCreated = DateTime.Now.AddDays(i),
                    User = user
                };

                user.Posts.Add(post);
                postFakeRepository.entities.Add(post);
            }

            userFakeRepository.entities.Add(user);

            PostsController postsController = new PostsController(
                postFakeRepository, userFakeRepository, threadFakeRepository, voteFakeRepository, commentFakeRepository);

            SetupController(postsController);

            var posts = postsController.GetAll(user.SessionKey).ToList<PostModel>();
            int expectedPostCount = 20;

            Assert.AreEqual(expectedPostCount, posts.Count);
        }

        [TestMethod]
        public void Get_GetAllTest_ValidExecution_CheckingForCorrectOrder()
        {
            FakeRepository<Post> postFakeRepository = new FakeRepository<Post>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();
            FakeRepository<Vote> voteFakeRepository = new FakeRepository<Vote>();
            FakeRepository<Comment> commentFakeRepository = new FakeRepository<Comment>();

            User user = new User()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "TestUsername",
                Nickname = "TestNickname",
                SessionKey = "0SuGqVGqRwitYtijDvHlVfHGotklitbwHdYFkgwIRcIQjRASPQ"
            };

            for (int i = 1; i <= 20; i++)
            {
                Post post = new Post()
                {
                    Content = string.Format("Test post content {0}", i),
                    DateCreated = DateTime.Now.AddDays(i),
                    User = user
                };

                user.Posts.Add(post);
                postFakeRepository.entities.Add(post);
            }

            userFakeRepository.entities.Add(user);

            PostsController postsController = new PostsController(
                postFakeRepository, userFakeRepository, threadFakeRepository, voteFakeRepository, commentFakeRepository);

            SetupController(postsController);

            var posts = postsController.GetAll(user.SessionKey).ToArray<PostModel>();

            for (int i = 0; i < posts.Length - 1; i++)
            {
                int result = posts[i].PostDate.CompareTo(posts[i + 1].PostDate);
                Assert.IsTrue(result > 0);
            }
        }

        [TestMethod]
        public void Get_GetPageTest_GettingLast5PostsFromTotalOf30()
        {
            FakeRepository<Post> postFakeRepository = new FakeRepository<Post>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();
            FakeRepository<Vote> voteFakeRepository = new FakeRepository<Vote>();
            FakeRepository<Comment> commentFakeRepository = new FakeRepository<Comment>();

            User user = new User()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "TestUsername",
                Nickname = "TestNickname",
                SessionKey = "0SuGqVGqRwitYtijDvHlVfHGotklitbwHdYFkgwIRcIQjRASPQ"
            };

            for (int i = 1; i <= 30; i++)
            {
                Post post = new Post()
                {
                    Content = string.Format("Test post content {0}", i),
                    DateCreated = DateTime.Now.AddDays(i),
                    User = user
                };

                user.Posts.Add(post);
                postFakeRepository.entities.Add(post);
            }

            userFakeRepository.entities.Add(user);

            PostsController postsController = new PostsController(
                postFakeRepository, userFakeRepository, threadFakeRepository, voteFakeRepository, commentFakeRepository);

            SetupController(postsController);

            var lastFivePosts = postsController.GetPage(5, 5, user.SessionKey).ToArray<PostModel>();
            int expectedPostCount = 5;

            Assert.AreEqual(expectedPostCount, lastFivePosts.Length);

            // Checking if the last 5 posts are the ones they should be.
            for (int i = 0; i < lastFivePosts.Length; i++)
            {
                string expectedPostContent = string.Format("Test post content {0}", lastFivePosts.Length - i);
                Assert.AreEqual(expectedPostContent, lastFivePosts[i].Content);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Post_CreatePostTest_InvalidSessionKey_NoSuchUserWithThatSessionKey()
        {
            FakeRepository<Post> postFakeRepository = new FakeRepository<Post>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();
            FakeRepository<Vote> voteFakeRepository = new FakeRepository<Vote>();
            FakeRepository<Comment> commentFakeRepository = new FakeRepository<Comment>();

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

            PostCreateModel model = new PostCreateModel()
            {
                Content = "Test post content",
                PostDate = DateTime.Now,
                ThreadId = thread.Id
            };

            userFakeRepository.entities.Add(user);
            threadFakeRepository.entities.Add(thread);

            PostsController postsController = new PostsController(
                postFakeRepository, userFakeRepository, threadFakeRepository, voteFakeRepository, commentFakeRepository);

            SetupController(postsController);

            var response = postsController.CreatePost(model, "1InvalidSessionKeyvHlVfHGotklitbwHdYFkgwIRcIQjRAPQ");
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Post_CreatePostTest_InvalidThreadId()
        {
            FakeRepository<Post> postFakeRepository = new FakeRepository<Post>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();
            FakeRepository<Vote> voteFakeRepository = new FakeRepository<Vote>();
            FakeRepository<Comment> commentFakeRepository = new FakeRepository<Comment>();

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

            PostCreateModel model = new PostCreateModel()
            {
                Content = "Test post content",
                PostDate = DateTime.Now,
                ThreadId = 100 // Invalid Id.
            };

            userFakeRepository.entities.Add(user);
            threadFakeRepository.entities.Add(thread);

            PostsController postsController = new PostsController(
                postFakeRepository, userFakeRepository, threadFakeRepository, voteFakeRepository, commentFakeRepository);

            SetupController(postsController);

            var response = postsController.CreatePost(model, user.SessionKey);
        }

        [TestMethod]
        public void Post_CreatePostTest_ValidCreation()
        {
            FakeRepository<Post> postFakeRepository = new FakeRepository<Post>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();
            FakeRepository<Vote> voteFakeRepository = new FakeRepository<Vote>();
            FakeRepository<Comment> commentFakeRepository = new FakeRepository<Comment>();

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

            PostCreateModel model = new PostCreateModel()
            {
                Content = "Test post content",
                PostDate = DateTime.Now,
                ThreadId = thread.Id
            };

            userFakeRepository.entities.Add(user);
            threadFakeRepository.entities.Add(thread);

            PostsController postsController = new PostsController(
                postFakeRepository, userFakeRepository, threadFakeRepository, voteFakeRepository, commentFakeRepository);

            SetupController(postsController);

            var response = postsController.CreatePost(model, user.SessionKey);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Post_VoteForPostTest_InvalidSessionKey_NoUserWithSuchSessionKey()
        {
            FakeRepository<Post> postFakeRepository = new FakeRepository<Post>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();
            FakeRepository<Vote> voteFakeRepository = new FakeRepository<Vote>();
            FakeRepository<Comment> commentFakeRepository = new FakeRepository<Comment>();

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

            Post post = new Post()
            {
                Content = "Test post content",
                DateCreated = DateTime.Now,
                User = user,
                Thread = thread
            };

            VoteModel voteModel = new VoteModel()
            {
                Value = 5
            };

            userFakeRepository.entities.Add(user);
            threadFakeRepository.entities.Add(thread);
            postFakeRepository.entities.Add(post);

            PostsController postsController = new PostsController(
                postFakeRepository, userFakeRepository, threadFakeRepository, voteFakeRepository, commentFakeRepository);

            SetupController(postsController);

            var response = postsController.VoteForPost(voteModel, post.Id, "1InvalidSessionKeyvHlVfHGotklitbwHdYFkgwIRcIQjRAPQ");
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Post_VoteForPostTest_InvalidPostId()
        {
            FakeRepository<Post> postFakeRepository = new FakeRepository<Post>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();
            FakeRepository<Vote> voteFakeRepository = new FakeRepository<Vote>();
            FakeRepository<Comment> commentFakeRepository = new FakeRepository<Comment>();

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

            Post post = new Post()
            {
                Content = "Test post content",
                DateCreated = DateTime.Now,
                User = user,
                Thread = thread
            };

            VoteModel voteModel = new VoteModel()
            {
                Value = 5
            };

            userFakeRepository.entities.Add(user);
            threadFakeRepository.entities.Add(thread);
            postFakeRepository.entities.Add(post);

            PostsController postsController = new PostsController(
                postFakeRepository, userFakeRepository, threadFakeRepository, voteFakeRepository, commentFakeRepository);

            SetupController(postsController);

            int invalidPostId = 100;
            var response = postsController.VoteForPost(voteModel, invalidPostId, user.SessionKey);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Post_VoteForPostTest_InvalidVoteValue_ValueTooLarge()
        {
            FakeRepository<Post> postFakeRepository = new FakeRepository<Post>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();
            FakeRepository<Vote> voteFakeRepository = new FakeRepository<Vote>();
            FakeRepository<Comment> commentFakeRepository = new FakeRepository<Comment>();

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

            Post post = new Post()
            {
                Content = "Test post content",
                DateCreated = DateTime.Now,
                User = user,
                Thread = thread
            };

            VoteModel voteModel = new VoteModel()
            {
                Value = 20 // Invalid vote value, too large.
            };

            userFakeRepository.entities.Add(user);
            threadFakeRepository.entities.Add(thread);
            postFakeRepository.entities.Add(post);

            PostsController postsController = new PostsController(
                postFakeRepository, userFakeRepository, threadFakeRepository, voteFakeRepository, commentFakeRepository);

            SetupController(postsController);

            int invalidPostId = 100;
            var response = postsController.VoteForPost(voteModel, invalidPostId, user.SessionKey);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Post_VoteForPostTest_InvalidVoteValue_NegativeValue()
        {
            FakeRepository<Post> postFakeRepository = new FakeRepository<Post>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();
            FakeRepository<Vote> voteFakeRepository = new FakeRepository<Vote>();
            FakeRepository<Comment> commentFakeRepository = new FakeRepository<Comment>();

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

            Post post = new Post()
            {
                Content = "Test post content",
                DateCreated = DateTime.Now,
                User = user,
                Thread = thread
            };

            VoteModel voteModel = new VoteModel()
            {
                Value = -1 // Invalid vote value.
            };

            userFakeRepository.entities.Add(user);
            threadFakeRepository.entities.Add(thread);
            postFakeRepository.entities.Add(post);

            PostsController postsController = new PostsController(
                postFakeRepository, userFakeRepository, threadFakeRepository, voteFakeRepository, commentFakeRepository);

            SetupController(postsController);

            int invalidPostId = 100;
            var response = postsController.VoteForPost(voteModel, invalidPostId, user.SessionKey);
        }

        [TestMethod]
        public void Post_VoteForPostTest_ValidExecutionOfTheMethod()
        {
            FakeRepository<Post> postFakeRepository = new FakeRepository<Post>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();
            FakeRepository<Vote> voteFakeRepository = new FakeRepository<Vote>();
            FakeRepository<Comment> commentFakeRepository = new FakeRepository<Comment>();

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

            Post post = new Post()
            {
                Content = "Test post content",
                DateCreated = DateTime.Now,
                User = user,
                Thread = thread
            };

            VoteModel voteModel = new VoteModel()
            {
                Value = 5
            };

            userFakeRepository.entities.Add(user);
            threadFakeRepository.entities.Add(thread);
            postFakeRepository.entities.Add(post);

            PostsController postsController = new PostsController(
                postFakeRepository, userFakeRepository, threadFakeRepository, voteFakeRepository, commentFakeRepository);

            SetupController(postsController);

            var response = postsController.VoteForPost(voteModel, post.Id, user.SessionKey);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Post_CommentForPostTest_InvalidSessionKey_NoUserWithSuchSessionKey()
        {
            FakeRepository<Post> postFakeRepository = new FakeRepository<Post>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();
            FakeRepository<Vote> voteFakeRepository = new FakeRepository<Vote>();
            FakeRepository<Comment> commentFakeRepository = new FakeRepository<Comment>();

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

            Post post = new Post()
            {
                Content = "Test post content",
                DateCreated = DateTime.Now,
                User = user,
                Thread = thread
            };

            CommentModel commentModel = new CommentModel()
            {
                Content = "Just a test comment.",
                CommentDate = DateTime.Now
            };

            userFakeRepository.entities.Add(user);
            threadFakeRepository.entities.Add(thread);
            postFakeRepository.entities.Add(post);

            PostsController postsController = new PostsController(
                postFakeRepository, userFakeRepository, threadFakeRepository, voteFakeRepository, commentFakeRepository);

            SetupController(postsController);

            var response = postsController.CommentForPost(commentModel, post.Id, "1InvalidSessionKeyvHlVfHGotklitbwHdYFkgwIRcIQjRAPQ");
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Post_CommentForPostTest_InvalidPostId()
        {
            FakeRepository<Post> postFakeRepository = new FakeRepository<Post>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();
            FakeRepository<Vote> voteFakeRepository = new FakeRepository<Vote>();
            FakeRepository<Comment> commentFakeRepository = new FakeRepository<Comment>();

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

            Post post = new Post()
            {
                Content = "Test post content",
                DateCreated = DateTime.Now,
                User = user,
                Thread = thread
            };

            CommentModel commentModel = new CommentModel()
            {
                Content = "Just a test comment.",
                CommentDate = DateTime.Now
            };

            userFakeRepository.entities.Add(user);
            threadFakeRepository.entities.Add(thread);
            postFakeRepository.entities.Add(post);

            PostsController postsController = new PostsController(
                postFakeRepository, userFakeRepository, threadFakeRepository, voteFakeRepository, commentFakeRepository);

            SetupController(postsController);

            int invalidPostId = 100;
            var response = postsController.CommentForPost(commentModel, invalidPostId, user.SessionKey);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Post_CommentForPostTest_InvalidCommentContent_ContentCannotBeNull()
        {
            FakeRepository<Post> postFakeRepository = new FakeRepository<Post>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();
            FakeRepository<Vote> voteFakeRepository = new FakeRepository<Vote>();
            FakeRepository<Comment> commentFakeRepository = new FakeRepository<Comment>();

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

            Post post = new Post()
            {
                Content = "Test post content",
                DateCreated = DateTime.Now,
                User = user,
                Thread = thread
            };

            CommentModel commentModel = new CommentModel()
            {
                Content = null,
                CommentDate = DateTime.Now
            };

            userFakeRepository.entities.Add(user);
            threadFakeRepository.entities.Add(thread);
            postFakeRepository.entities.Add(post);

            PostsController postsController = new PostsController(
                postFakeRepository, userFakeRepository, threadFakeRepository, voteFakeRepository, commentFakeRepository);

            SetupController(postsController);

            var response = postsController.CommentForPost(commentModel, post.Id, user.SessionKey);
        }

        [TestMethod]
        public void Post_CommentForPostTest_ValidExecutionOfTheMethod()
        {
            FakeRepository<Post> postFakeRepository = new FakeRepository<Post>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();
            FakeRepository<Vote> voteFakeRepository = new FakeRepository<Vote>();
            FakeRepository<Comment> commentFakeRepository = new FakeRepository<Comment>();

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

            Post post = new Post()
            {
                Content = "Test post content",
                DateCreated = DateTime.Now,
                User = user,
                Thread = thread
            };

            CommentModel commentModel = new CommentModel()
            {
                Content = "Just some test content",
                CommentDate = DateTime.Now
            };

            userFakeRepository.entities.Add(user);
            threadFakeRepository.entities.Add(thread);
            postFakeRepository.entities.Add(post);

            PostsController postsController = new PostsController(
                postFakeRepository, userFakeRepository, threadFakeRepository, voteFakeRepository, commentFakeRepository);

            SetupController(postsController);

            var response = postsController.CommentForPost(commentModel, post.Id, user.SessionKey);
            int expectedCommentCount = 1;

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual(expectedCommentCount, commentFakeRepository.entities.Count);
        }
    }
}
