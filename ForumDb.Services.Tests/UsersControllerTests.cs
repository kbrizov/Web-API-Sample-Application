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
    public class UsersControllerTests
    {
        private void SetupController(ApiController controller)
        {
            var config = new HttpConfiguration();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/users");
            var route = config.Routes.MapHttpRoute(
                name: "UsersApi",
                routeTemplate: "api/users/{action}",
                defaults: new
                {
                    controller = "users",
                }
            );

            var routeData = new HttpRouteData(route);
            routeData.Values.Add("controller", "users");
            controller.ControllerContext = new HttpControllerContext(config, routeData, request);
            controller.Request = request;
            controller.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
            controller.Request.Properties[HttpPropertyKeys.HttpRouteDataKey] = routeData;
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Post_RegisterUser_InvalidUsername_UsernameIsNull()
        {
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            UsersController usersController = new UsersController(userFakeRepository);
            SetupController(usersController);

            UserModel model = new UserModel()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = null,
                Nickname = "TestNickname"
            };

            usersController.RegisterUser(model);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Post_RegisterUser_InvalidUsername_UsernameIsTooShort()
        {
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            UsersController usersController = new UsersController(userFakeRepository);
            SetupController(usersController);

            UserModel model = new UserModel()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "User",
                Nickname = "TestNickname"
            };

            usersController.RegisterUser(model);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Post_RegisterUser_InvalidUsername_UsernameIsTooLarge()
        {
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            UsersController usersController = new UsersController(userFakeRepository);
            SetupController(usersController);

            UserModel model = new UserModel()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "User012345678901234567890123456789",
                Nickname = "TestNickname"
            };

            usersController.RegisterUser(model);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Post_RegisterUser_InvalidUsername_ContainsUnallowedSymbols()
        {
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            UsersController usersController = new UsersController(userFakeRepository);
            SetupController(usersController);

            UserModel model = new UserModel()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "User@#",
                Nickname = "TestNickname"
            };

            usersController.RegisterUser(model);
        }

        [TestMethod]
        public void Post_RegisterUser_UsernameIsValidAndExactlyMinSizeLong_UserShouldBeRegisterSuccessfully()
        {
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            UsersController usersController = new UsersController(userFakeRepository);
            SetupController(usersController);

            UserModel model = new UserModel()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "user12",
                Nickname = "TestNickname"
            };

            var response = usersController.RegisterUser(model);

            int expectedCount = 1;
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual(expectedCount, userFakeRepository.entities.Count);
        }

        [TestMethod]
        public void Post_RegisterUser_UsernameIsValidAndExactlyMaxSizeLong_UserShouldBeRegisterSuccessfully()
        {
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            UsersController usersController = new UsersController(userFakeRepository);
            SetupController(usersController);

            UserModel model = new UserModel()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "user01234567890123456789012345",
                Nickname = "TestNickname"
            };

            var response = usersController.RegisterUser(model);

            int expectedCount = 1;
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual(expectedCount, userFakeRepository.entities.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Post_RegisterUser_InvalidNickname_NicknameIsNull()
        {
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            UsersController usersController = new UsersController(userFakeRepository);
            SetupController(usersController);

            UserModel model = new UserModel()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "TestUsername",
                Nickname = null
            };

            usersController.RegisterUser(model);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Post_RegisterUser_InvalidNickname_NicknameIsTooShort()
        {
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            UsersController usersController = new UsersController(userFakeRepository);
            SetupController(usersController);

            UserModel model = new UserModel()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "TestUsername",
                Nickname = "Nick"
            };

            usersController.RegisterUser(model);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Post_RegisterUser_InvalidNickname_NicknameIsTooLarge()
        {
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            UsersController usersController = new UsersController(userFakeRepository);
            SetupController(usersController);

            UserModel model = new UserModel()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "Nickname012345678901234567890123456789",
                Nickname = "TestNickname"
            };

            usersController.RegisterUser(model);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Post_RegisterUser_InvalidNickname_ContainsUnallowedSymbols()
        {
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            UsersController usersController = new UsersController(userFakeRepository);
            SetupController(usersController);

            UserModel model = new UserModel()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "TestUsername",
                Nickname = "TestNickname@#$%^"
            };

            usersController.RegisterUser(model);
        }

        [TestMethod]
        public void Post_RegisterUser_NicknameIsValidAndExactlyMinSizeLong_UserShouldBeRegisterSuccessfully()
        {
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            UsersController usersController = new UsersController(userFakeRepository);
            SetupController(usersController);

            UserModel model = new UserModel()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "TestUsername",
                Nickname = "Nickna"
            };

            var response = usersController.RegisterUser(model);

            int expectedCount = 1;
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual(expectedCount, userFakeRepository.entities.Count);
        }

        [TestMethod]
        public void Post_RegisterUser_NicknameIsValidAndExactlyMaxSizeLong_UserShouldBeRegisterSuccessfully()
        {
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            UsersController usersController = new UsersController(userFakeRepository);
            SetupController(usersController);

            UserModel model = new UserModel()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "TestUsername",
                Nickname = "Nickname0123456789012345678912"
            };

            var response = usersController.RegisterUser(model);

            int expectedCount = 1;
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual(expectedCount, userFakeRepository.entities.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Post_RegisterUser_InvalidAuthCode_AuthCodeTooShort()
        {
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            UsersController usersController = new UsersController(userFakeRepository);
            SetupController(usersController);

            UserModel model = new UserModel()
            {
                AuthCode = "0123456789",
                Username = "TestUsername",
                Nickname = "TestNickname"
            };

            usersController.RegisterUser(model);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Post_RegisterUser_InvalidAuthCode_AuthCodeTooLarge()
        {
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            UsersController usersController = new UsersController(userFakeRepository);
            SetupController(usersController);

            UserModel model = new UserModel()
            {
                AuthCode = "01234567890123456789012345678901234567890123456789",
                Username = "TestUsername",
                Nickname = "TestNickname"
            };

            usersController.RegisterUser(model);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Post_RegisterUser_InvalidUsername_UsernameAlreadyExists()
        {
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            UsersController usersController = new UsersController(userFakeRepository);
            SetupController(usersController);

            User user = new User()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "TestUsername",
                Nickname = "TestNickname",
                SessionKey = "sessionKey0123456789012345678901234567890123456789"
            };

            userFakeRepository.entities.Add(user);

            UserModel model = new UserModel()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "TestUsername",
                Nickname = "TestNickname"
            };

            usersController.RegisterUser(model);
        }

        [TestMethod]
        public void Post_RegisterUser_ValidUsernameAndPassword_ShouldRegisterSuccessfully()
        {
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            UsersController usersController = new UsersController(userFakeRepository);
            SetupController(usersController);

            UserModel model = new UserModel()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "TestUsername",
                Nickname = "TestNickname"
            };

            var response = usersController.RegisterUser(model);

            int expectedCount = 1;
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual(expectedCount, userFakeRepository.entities.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Post_LoginUser_InvalidUsername_UsernameIsNull()
        {
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            UsersController usersController = new UsersController(userFakeRepository);
            SetupController(usersController);

            UserModel model = new UserModel()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = null,
                Nickname = "TestNickname"
            };

            usersController.RegisterUser(model);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Post_LoginUser_InvalidUsername_UsernameIsTooShort()
        {
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            UsersController usersController = new UsersController(userFakeRepository);
            SetupController(usersController);

            UserModel model = new UserModel()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "User",
                Nickname = "TestNickname"
            };

            usersController.RegisterUser(model);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Post_LoginUser_InvalidUsername_UsernameIsTooLarge()
        {
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            UsersController usersController = new UsersController(userFakeRepository);
            SetupController(usersController);

            UserModel model = new UserModel()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "User012345678901234567890123456789",
                Nickname = "TestNickname"
            };

            usersController.RegisterUser(model);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Post_LoginUser_InvalidUsername_ContainsUnallowedSymbols()
        {
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            UsersController usersController = new UsersController(userFakeRepository);
            SetupController(usersController);

            UserModel model = new UserModel()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "User@#",
                Nickname = "TestNickname"
            };

            usersController.RegisterUser(model);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Post_LoginUser_InvalidAuthCode_AuthCodeTooShort()
        {
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            UsersController usersController = new UsersController(userFakeRepository);
            SetupController(usersController);

            UserModel model = new UserModel()
            {
                AuthCode = "0123456789",
                Username = "TestUsername",
                Nickname = "TestNickname"
            };

            usersController.RegisterUser(model);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Post_LoginUser_InvalidAuthCode_AuthCodeTooLarge()
        {
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            UsersController usersController = new UsersController(userFakeRepository);
            SetupController(usersController);

            UserModel model = new UserModel()
            {
                AuthCode = "01234567890123456789012345678901234567890123456789",
                Username = "TestUsername",
                Nickname = "TestNickname"
            };

            usersController.RegisterUser(model);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Post_LoginUser_InvalidLogin_NoUserWithSuchAuthCode()
        {
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            UsersController usersController = new UsersController(userFakeRepository);
            SetupController(usersController);

            User user = new User()
            {
                AuthCode = "9876543210987654321098765432109876543210",
                Username = "TestUsername",
                Nickname = "TestNickname",
            };

            userFakeRepository.entities.Add(user);

            UserModel model = new UserModel()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "TestUsername",
                Nickname = "TestNickname"
            };

            var response = usersController.LoginUser(model);
        }

        [TestMethod]
        public void Post_LoginUser_ValidLogin()
        {
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            UsersController usersController = new UsersController(userFakeRepository);
            SetupController(usersController);

            User user = new User()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "TestUsername",
                Nickname = "TestNickname",
                SessionKey = "0SuGqVGqRwitYtijDvHlVfHGotklitbwHdYFkgwIRcIQjRASPQ"
            };

            userFakeRepository.entities.Add(user);

            UserModel model = new UserModel()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "TestUsername",
                Nickname = "TestNickname"
            };

            var response = usersController.LoginUser(model);

            int expectedCount = 1;
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(expectedCount, userFakeRepository.entities.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpResponseException))]
        public void Post_LogoutUser_InvalidLogout_NoUserWithSuchSessionKey()
        {
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            UsersController usersController = new UsersController(userFakeRepository);
            SetupController(usersController);

            User user = new User()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "TestUsername",
                Nickname = "TestNickname",
                SessionKey = "0RasasdasdGagsdSAjDvHlVfHGotklitbwHdYFkgwIRcIQjBAs"
            };

            userFakeRepository.entities.Add(user);

            UserLoggedModel loggedModel = new UserLoggedModel()
            {
                Nickname = "TestNickname",
                SessionKey = "1NoUserWithThatSessionKeyInvalidSessionKeyBlaQjRAS"
            };

            var response = usersController.LogoutUser(loggedModel);
        }

        [TestMethod]
        public void Post_LogoutUser_ValidLogout()
        {
            FakeRepository<User> userFakeRepository = new FakeRepository<User>();
            UsersController usersController = new UsersController(userFakeRepository);
            SetupController(usersController);

            User user = new User()
            {
                AuthCode = "0123456789012345678901234567890123456789",
                Username = "TestUsername",
                Nickname = "TestNickname",
                SessionKey = "0SuGqVGqRwitYtijDvHlVfHGotklitbwHdYFkgwIRcIQjRASPQ"
            };

            userFakeRepository.entities.Add(user);

            UserLoggedModel loggedModel = new UserLoggedModel()
            {
                Nickname = "TestNickname",
                SessionKey = "0SuGqVGqRwitYtijDvHlVfHGotklitbwHdYFkgwIRcIQjRASPQ"
            };

            var response = usersController.LogoutUser(loggedModel);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNull(user.SessionKey);
        }
    }
}
