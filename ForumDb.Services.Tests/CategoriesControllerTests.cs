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
    public class CategoriesControllerTests
    {
        private List<Route> GetRoutes()
        {
            List<Route> routeList = new List<Route>();

            Route defaultApiRoute = new Route(
                "DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });

            Route categoriesApiRoute = new Route(
                "CategoriesApi", "api/categories/{action}", new { controller = "categories", action = "create" });

            routeList.Add(defaultApiRoute);
            routeList.Add(categoriesApiRoute);

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

            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/categories");
            request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, config);
            
            controller.Configuration = config;
            controller.Request = request;
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void PostRequest_CreateCategoryTest_InvalidCreation_CannotAddNullCategory()
        {
            FakeRepository<Category> categoryFakeRepository = new FakeRepository<Category>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();

            string sessionKey = "01234567890123456789012345678901234567890123456789";

            User user = new User()
            {
                SessionKey = sessionKey
            };

            userFakeRepository.Add(user);

            CategoryModel categoryModel = null;

            CategoriesController categoriesController = new CategoriesController(
                categoryFakeRepository, userFakeRepository, threadFakeRepository);

            SetupController(categoriesController);

            categoriesController.CreateCategory(categoryModel, sessionKey);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void PostRequest_CreateCategoryTest_InvalidCategoryName()
        {
            FakeRepository<Category> categoryFakeRepository = new FakeRepository<Category>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();

            string sessionKey = "01234567890123456789012345678901234567890123456789";

            CategoryModel categoryModel = new CategoryModel()
            {
                Name = null
            };

            User user = new User()
            {
                SessionKey = sessionKey
            };

            userFakeRepository.Add(user);

            CategoriesController categoriesController = new CategoriesController(
                categoryFakeRepository, userFakeRepository, threadFakeRepository);

            SetupController(categoriesController);

            var response = categoriesController.CreateCategory(categoryModel, sessionKey);

            int expectedCategoryCount = 1;
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            Assert.AreEqual(expectedCategoryCount, categoryFakeRepository.entities.Count);
        }

        [TestMethod]
        public void PostRequest_CreateCategoryTest_SuccessfulCreation()
        {
            FakeRepository<Category> categoryFakeRepository = new FakeRepository<Category>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();

            string sessionKey = "01234567890123456789012345678901234567890123456789";

            CategoryModel categoryModel = new CategoryModel()
            {
                Name = "Test category"
            };

            User user = new User()
            {
                SessionKey = sessionKey
            };

            userFakeRepository.Add(user);

            CategoriesController categoriesController = new CategoriesController(
                categoryFakeRepository, userFakeRepository, threadFakeRepository);

            SetupController(categoriesController);

            var response = categoriesController.CreateCategory(categoryModel, sessionKey);

            int expectedCategoryCount = 1;
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            Assert.AreEqual(expectedCategoryCount, categoryFakeRepository.entities.Count);
        }

        [TestMethod]
        public void GetRequest_GetAllTest_OneCategoryOnly()
        {
            FakeRepository<Category> categoryFakeRepository = new FakeRepository<Category>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();

            string sessionKey = "01234567890123456789012345678901234567890123456789";

            Category category = new Category()
            {
                Name = "Test category"
            };

            categoryFakeRepository.entities.Add(category);

            User user = new User()
            {
                SessionKey = sessionKey
            };

            userFakeRepository.Add(user);

            CategoriesController categoriesController = new CategoriesController(
                categoryFakeRepository, userFakeRepository, threadFakeRepository);

            SetupController(categoriesController);

            var allCategories = categoriesController.GetAll(sessionKey).ToList<string>();

            int expectedCategoryCount = 1;
            Assert.AreEqual(expectedCategoryCount, allCategories.Count);
        }

        [TestMethod]
        public void GetRequest_GetAllTest_NoCategories()
        {
            FakeRepository<Category> categoryFakeRepository = new FakeRepository<Category>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();

            string sessionKey = "01234567890123456789012345678901234567890123456789";

            User user = new User()
            {
                SessionKey = sessionKey
            };

            userFakeRepository.Add(user);

            CategoriesController categoriesController = new CategoriesController(
                categoryFakeRepository, userFakeRepository, threadFakeRepository);

            SetupController(categoriesController);

            var allCategories = categoriesController.GetAll(sessionKey).ToList<string>();

            int expectedCategoryCount = 0;
            Assert.AreEqual(expectedCategoryCount, allCategories.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetRequest_GetAllTest_InvalidSessionKey_NoUserWithSuchKey()
        {
            FakeRepository<Category> categoryFakeRepository = new FakeRepository<Category>();
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            FakeRepository<Thread> threadFakeRepository = new FakeRepository<Thread>();

            string sessionKey = "01234567890123456789012345678901234567890123456789";

            Category category = new Category()
            {
                Name = "Test category"
            };

            categoryFakeRepository.entities.Add(category);

            CategoriesController categoriesController = new CategoriesController(
                categoryFakeRepository, userFakeRepository, threadFakeRepository);

            SetupController(categoriesController);

            var allCategories = categoriesController.GetAll(sessionKey).ToList<string>();
        }
    }
}
