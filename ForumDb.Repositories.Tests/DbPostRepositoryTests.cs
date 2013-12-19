using System;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using ForumDb.Data;
using ForumDb.Models;

namespace ForumDb.Repositories.Tests
{
    [TestClass]
    public class DbPostRepositoryTests
    {
        private DbContext dbContext;
        private DbSet<Post> postEntities;
        private IRepository<Post> postRepository;
        private static TransactionScope transactionScope;

        public DbPostRepositoryTests()
        {
            this.dbContext = new ForumDbContext();
            this.postRepository = new DbPostRepository(this.dbContext);
            this.postEntities = this.dbContext.Set<Post>();
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
            Post entity = null;
            this.postRepository.Add(entity);
        }

        [TestMethod]
        public void AddTest_EntityIsValid_ShouldBeAddedSuccessfuly()
        {
            Post entity = new Post()
            {
                Content = "Test content",
                DateCreated = DateTime.Now
            };

            this.postRepository.Add(entity);
            var foundEntity = this.postEntities.Find(entity.Id);

            Assert.IsTrue(entity.Id != 0);
            Assert.IsNotNull(foundEntity);
            Assert.AreSame(entity, foundEntity);
        }

        [TestMethod]
        public void GetByIdTest_InvalidId_ShouldReturnNull()
        {
            int invalidId = -1;
            var entity = this.postRepository.GetById(invalidId);

            Assert.IsNull(entity);
        }

        [TestMethod]
        public void GetByIdTest_ValidId_ShouldReturnValidEntity()
        {
            var entity = new Post()
            {
                Content = "Test content",
                DateCreated = DateTime.Now
            };

            this.postRepository.Add(entity);
            var foundEntity = this.postEntities.Find(entity.Id);

            Assert.AreSame(entity, foundEntity);
        }

        [TestMethod]
        public void UpdateTest_InvalidEntityId()
        {
            int invalidId = -1;
            var entity = new Post()
            {
                Content = "Test content",
                DateCreated = DateTime.Now
            };

            this.postRepository.Add(entity);

            Post postToUpgradeWith = new Post()
            {
                Content = "Updated test content",
                DateCreated = DateTime.Now.AddDays(1)
            };

            Post upgradedPost = this.postRepository.Update(invalidId, postToUpgradeWith);

            Assert.IsNull(upgradedPost);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateTest_InvalidUpdateEntity_ShouldThrowArgumentNullException()
        {
            Post entity = new Post()
            {
                Content = "Test content",
                DateCreated = DateTime.Now
            };

            this.postRepository.Add(entity);

            Post postToUpgradeWith = null;

            Post upgradedPost = this.postRepository.Update(entity.Id, postToUpgradeWith);
        }

        [TestMethod]
        public void UpdateTest_ValidUpdate()
        {
            Post entity = new Post()
            {
                Content = "Test content",
                DateCreated = DateTime.Now
            };

            this.postRepository.Add(entity);

            Post postToUpgradeWith = new Post()
            {
                Content = "Updated test content",
                DateCreated = DateTime.Now.AddDays(1)
            };

            Post upgradedPost = this.postRepository.Update(entity.Id, postToUpgradeWith);

            Assert.AreSame(entity, upgradedPost);
            Assert.AreEqual(upgradedPost.Content, postToUpgradeWith.Content);
        }
    }
}
