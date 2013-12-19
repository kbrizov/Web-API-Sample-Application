using ForumDb.Models;
using ForumDb.Repositories;
using ForumDb.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace ForumDb.Services.Controllers
{
    public class UsersController : BaseApiController
    {
        private const int MinUsernameLength = 6;
        private const int MaxUsernameLength = 30;

        private const int MinNicknameLength = 6;
        private const int MaxNicknameLength = 30;

        private const string ValidUsernameChars =
           "qwertyuioplkjhgfdsazxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM1234567890_.";

        private const string ValidNicknameChars =
            "qwertyuioplkjhgfdsazxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM1234567890_. -";

        private const string SessionKeyChars =
            "qwertyuioplkjhgfdsazxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM";

        private static readonly Random randomGenerator = new Random();

        private const int SessionKeyLength = 50;

        private const int Sha1Length = 40;

        private readonly IRepository<User> userRepository;

        public UsersController(IRepository<User> userRepository)
        {
            this.userRepository = userRepository;
        }

        [HttpPost]
        [ActionName("register")]
        public HttpResponseMessage RegisterUser(UserModel model)
        {
            var responseMessage = this.PerformOperationAndHandleExceptions(() =>
                {
                    ValidateUsername(model.Username);
                    ValidateNickname(model.Nickname);
                    ValidateAuthCode(model.AuthCode);

                    string modelUsernameToLower = model.Username.ToLower();
                    string modelNicknameToLower = model.Nickname.ToLower();

                    User user = this.userRepository.GetAll().Where(
                        usr => usr.Username.ToLower() == modelUsernameToLower &&
                               usr.AuthCode == model.AuthCode).FirstOrDefault();

                    if (user != null)
                    {
                        throw new InvalidOperationException("The username already exists.");
                    }

                    user = new User
                    {
                        Username = model.Username,
                        Nickname = model.Nickname,
                        AuthCode = model.AuthCode
                    };

                    this.userRepository.Add(user);
                    user.SessionKey = this.GenerateSessionKey(user.Id);
                    this.userRepository.Update(user.Id, user);

                    var userLoggedModel = new UserLoggedModel()
                    {
                        Nickname = user.Nickname,
                        SessionKey = user.SessionKey
                    };

                    var response = this.Request.CreateResponse(HttpStatusCode.Created, userLoggedModel);
                    return response;
                });

            return responseMessage;
        }

        [HttpPost]
        [ActionName("login")]
        public HttpResponseMessage LoginUser(UserModel model)
        {
            var responseMessage = this.PerformOperationAndHandleExceptions(() => 
                {
                    this.ValidateUsername(model.Username);
                    this.ValidateAuthCode(model.AuthCode);

                    string modelUsernameToLower = model.Username.ToLower();

                    User user = this.userRepository.GetAll().Where(
                        usr => usr.Username.ToLower() == modelUsernameToLower &&
                               usr.AuthCode == model.AuthCode).FirstOrDefault();

                    if (user == null)
                    {
                        throw new InvalidOperationException("Invalid username or password.");
                    }

                    if (user.SessionKey == null)
                    {
                        user.SessionKey = this.GenerateSessionKey(user.Id);
                        this.userRepository.Update(user.Id, user);
                    }

                    var userLoggedModel = new UserLoggedModel
                    {
                        Nickname = user.Nickname,
                        SessionKey = user.SessionKey
                    };

                    var response = this.Request.CreateResponse(HttpStatusCode.OK, userLoggedModel);
                    return response;
                });

            return responseMessage;
        }

        [HttpPut]
        [ActionName("logout")]
        public HttpResponseMessage LogoutUser(UserLoggedModel model)
        {
            var responseMessage = this.PerformOperationAndHandleExceptions(() =>
                {
                    var user = this.userRepository.GetAll().Where(
                        usr => usr.SessionKey == model.SessionKey).FirstOrDefault();

                    if (user == null)
                    {
                        throw new InvalidOperationException("The user is not logged in.");
                    }

                    user.SessionKey = null;
                    this.userRepository.Update(user.Id, user);

                    var response = this.Request.CreateResponse(HttpStatusCode.OK, (object)null);
                    return response;
                });

            return responseMessage;
        }

        private string GenerateSessionKey(int userId)
        {
            StringBuilder sessionKeyBuilder = new StringBuilder(SessionKeyLength);
            sessionKeyBuilder.Append(userId.ToString());

            while (sessionKeyBuilder.Length < SessionKeyLength)
            {
                int index = randomGenerator.Next(SessionKeyLength);
                sessionKeyBuilder.Append(SessionKeyChars[index]);
            }

            return sessionKeyBuilder.ToString();
        }

        private void ValidateUsername(string username)
        {
            if (username == null)
            {
                throw new ArgumentNullException("The username cannot be null.");
            }
            else if (username.Length < MinUsernameLength)
            {
                throw new ArgumentException(
                    string.Format("The username cannot be less than {0} symbols", MinUsernameLength));
            }
            else if (username.Length > MaxUsernameLength)
            {
                throw new ArgumentException(
                    string.Format("The username cannot be more than {0} symbols", MaxUsernameLength));
            }
            else if (username.Any(ch => !ValidUsernameChars.Contains(ch)))
            {
                throw new ArgumentException(
                    "Username can contain only Latin letters, digits, ., _");
            }
        }

        private void ValidateNickname(string nickname)
        {
            if (nickname == null)
            {
                throw new ArgumentNullException("The nickname cannot be null.");
            }
            else if (nickname.Length < MinNicknameLength)
            {
                throw new ArgumentException(
                    string.Format("The nickname cannot be less than {0} symbols", MinNicknameLength));
            }
            else if (nickname.Length > MaxNicknameLength)
            {
                throw new ArgumentException(
                    string.Format("The nickname cannot be more than {0} symbols", MaxNicknameLength));
            }
            else if (nickname.Any(ch => !ValidUsernameChars.Contains(ch)))
            {
                throw new ArgumentException(
                    "Nickname can contain only Latin letters, digits, ., _");
            }
        }

        private void ValidateAuthCode(string authCode)
        {
            if (authCode == null) 
            {
                throw new ArgumentNullException("The password (AuthCode) cannot be null.");
            }
            else if (authCode.Length != Sha1Length)
            {
                throw new ArgumentException("The password should be encrypted.");
            }
        }
    }
}
