using System;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using ForumDb.Data;
using ForumDb.Models;

namespace ForumDb.Repositories.Tests
{
    [TestClass]
    public class DbThreadRepositoryTests
    {
        private DbContext dbContext;
        private DbSet<Thread> threadEntities;
        private IRepository<Thread> threadRepository;
        private static TransactionScope transactionScope;

        public DbThreadRepositoryTests()
        {
            this.dbContext = new ForumDbContext();
            this.threadRepository = new DbThreadRepository(this.dbContext);
            this.threadEntities = this.dbContext.Set<Thread>();
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
            Thread entity = null;
            this.threadRepository.Add(entity);
        }

        [TestMethod]
        public void AddTest_EntityIsValid_ShouldBeAddedSuccessfuly()
        {
            Thread entity = new Thread()
            {
                Title = "Test title",
                Content = "Test content",
                DateCreated = DateTime.Now
            };

            this.threadRepository.Add(entity);
            var foundEntity = this.threadEntities.Find(entity.Id);

            Assert.IsTrue(entity.Id != 0);
            Assert.IsNotNull(foundEntity);
            Assert.AreSame(entity, foundEntity);
        }

        [TestMethod]
        public void GetByIdTest_InvalidId_ShouldReturnNull()
        {
            int invalidId = -1;
            var entity = this.threadRepository.GetById(invalidId);

            Assert.IsNull(entity);
        }

        [TestMethod]
        public void GetByIdTest_ValidId_ShouldReturnValidEntity()
        {
            Thread entity = new Thread()
            {
                Title = "Test title",
                Content = "Test content",
                DateCreated = DateTime.Now
            };

            this.threadRepository.Add(entity);
            var foundEntity = this.threadEntities.Find(entity.Id);

            Assert.AreSame(entity, foundEntity);
        }

        [TestMethod]
        public void UpdateTest_InvalidEntityId()
        {
            int invalidId = -1;
            Thread entity = new Thread()
            {
                Title = "Test title",
                Content = "Test content",
                DateCreated = DateTime.Now
            };

            this.threadRepository.Add(entity);

            Thread threadToUpgradeWith = new Thread()
            {
                Title = "Updated Test title",
                Content = "Test content",
                DateCreated = DateTime.Now.AddDays(1)
            };

            Thread upgradedThread = this.threadRepository.Update(invalidId, threadToUpgradeWith);

            Assert.IsNull(upgradedThread);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateTest_InvalidUpdateEntity_ShouldThrowArgumentNullException()
        {
            Thread entity = new Thread()
            {
                Title = "Test title",
                Content = "Test content",
                DateCreated = DateTime.Now
            };

            this.threadRepository.Add(entity);

            Thread threadToUpgradeWith = null;

            Thread upgradedThread = this.threadRepository.Update(entity.Id, threadToUpgradeWith);
        }

        [TestMethod]
        public void UpdateTest_ValidUpdate()
        {
            Thread entity = new Thread()
            {
                Title = "Test title",
                Content = "Test content",
                DateCreated = DateTime.Now
            };

            this.threadRepository.Add(entity);

            Thread threadToUpgradeWith = new Thread()
            {
                Title = "Updated Test title",
                Content = "Test content",
                DateCreated = DateTime.Now.AddDays(1)
            };

            Thread upgradedThread = this.threadRepository.Update(entity.Id, threadToUpgradeWith);

            Assert.AreSame(entity, upgradedThread);
            Assert.AreEqual(upgradedThread.Content, threadToUpgradeWith.Content);
        }
    }
}
