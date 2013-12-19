using System;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using ForumDb.Data;
using ForumDb.Models;

namespace ForumDb.Repositories.Tests
{
    [TestClass]
    public class DbUserRepositoryTests
    {
        private DbContext dbContext;
        private DbSet<User> userEntities;
        private IRepository<User> userRepository;
        private static TransactionScope transactionScope;

        public DbUserRepositoryTests()
        {
            this.dbContext = new ForumDbContext();
            this.userRepository = new DbUserRepository(this.dbContext);
            this.userEntities = this.dbContext.Set<User>();
        }

        [TestInitialize]
        public void TestInitiaze()
        {
            transactionScope = new TransactionScope();
        }

        [TestCleanup]
        public void TestTearDown()
        {
            transactionScope.Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddTest_EntityIsNull_ShouldThrowArgumentNullException()
        {
            User entity = null;
            this.userRepository.Add(entity);
        }

        [TestMethod]
        public void AddTest_EntityIsValid_ShouldBeAddedSuccessfuly()
        {
            User entity = new User()
            {
                Username = "TestUser",
                Nickname = "TheTestUser",
                AuthCode = "0123456789012345678901234567890123456789",
                SessionKey = "01234567890123456789012345678901234567890123456789"
            };

            this.userRepository.Add(entity);
            User foundEntity = this.userEntities.Find(entity.Id);

            Assert.IsTrue(entity.Id != 0);
            Assert.IsNotNull(foundEntity);
            Assert.AreSame(entity, foundEntity);
        }

        [TestMethod]
        public void GetByIdTest_InvalidId_ShouldReturnNull()
        {
            int invalidId = -1;
            var entity = this.userRepository.GetById(invalidId);

            Assert.IsNull(entity);
        }

        [TestMethod]
        public void GetByIdTest_ValidId_ShouldReturnValidEntity()
        {
            User entity = new User()
            {
                Username = "TestUser",
                Nickname = "TheTestUser",
                AuthCode = "0123456789012345678901234567890123456789",
                SessionKey = "01234567890123456789012345678901234567890123456789"
            };

            this.userRepository.Add(entity);
            User foundEntity = this.userEntities.Find(entity.Id);

            Assert.AreSame(entity, foundEntity);
        }

        [TestMethod]
        public void UpdateTest_InvalidEntityId()
        {
            int invalidId = -1;
            User entity = new User()
            {
                Username = "TestUser",
                Nickname = "TheTestUser",
                AuthCode = "0123456789012345678901234567890123456789",
                SessionKey = "01234567890123456789012345678901234567890123456789"
            };

            this.userRepository.Add(entity);

            User userToUpdateWith = new User()
            {
                Username = "UpdatedTestUser",
                Nickname = "UpdatedNickname",
                AuthCode = "UpdatedNickname5678901234567890123456789",
                SessionKey = "UpdatedNickname56789012345678901234567890123456789"
            };

            User updatedUser = this.userRepository.Update(invalidId, userToUpdateWith);

            Assert.IsNull(updatedUser);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateTest_InvalidUpdateEntity_ShouldThrowArgumentNullException()
        {
            User entity = new User()
            {
                Username = "TestUser",
                Nickname = "TheTestUser",
                AuthCode = "0123456789012345678901234567890123456789",
                SessionKey = "01234567890123456789012345678901234567890123456789"
            };

            this.userRepository.Add(entity);

            User userToUpdateWith = null;

            User updatedUser = this.userRepository.Update(entity.Id, userToUpdateWith);
        }

        [TestMethod]
        public void UpdateTest_ValidUpdate()
        {
            User entity = new User()
            {
                Username = "TestUser",
                Nickname = "TheTestUser",
                AuthCode = "0123456789012345678901234567890123456789",
                SessionKey = "01234567890123456789012345678901234567890123456789"
            };

            this.userRepository.Add(entity);

            User userToUpdateWith = new User()
            {
                Username = "UpdatedTestUser",
                Nickname = "UpdatedNickname",
                AuthCode = "UpdatedNickname5678901234567890123456789",
                SessionKey = "UpdatedNickname56789012345678901234567890123456789"
            };

            User updatedUser = this.userRepository.Update(entity.Id, userToUpdateWith);

            Assert.AreSame(entity, updatedUser);
            Assert.AreEqual(updatedUser.Nickname, userToUpdateWith.Nickname);
            Assert.AreEqual(updatedUser.Username, userToUpdateWith.Username);
            Assert.AreEqual(updatedUser.AuthCode, updatedUser.AuthCode);
        }
    }
}
