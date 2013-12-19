using System;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using ForumDb.Data;
using ForumDb.Models;

namespace ForumDb.Repositories.Tests
{
    [TestClass]
    public class DbCommentRepositoryTests
    {
        private DbContext dbContext;
        private DbSet<Comment> commentEntities;
        private IRepository<Comment> commentRepository;
        private static TransactionScope transactionScope;

        public DbCommentRepositoryTests()
        {
            this.dbContext = new ForumDbContext();
            this.commentRepository = new DbCommentRepository(this.dbContext);
            this.commentEntities = this.dbContext.Set<Comment>();
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
            Comment entity = null;
            this.commentRepository.Add(entity);
        }

        [TestMethod]
        public void AddTest_EntityIsValid_ShouldBeAddedSuccessfuly()
        {
            Comment entity = new Comment()
            {
                Content = "Test content",
                DateCreated = DateTime.Now
            };

            this.commentRepository.Add(entity);
            var foundEntity = this.commentEntities.Find(entity.Id);

            Assert.IsTrue(entity.Id != 0);
            Assert.IsNotNull(foundEntity);
            Assert.AreSame(entity, foundEntity);
        }

        [TestMethod]
        public void GetByIdTest_InvalidId_ShouldReturnNull()
        {
            int invalidId = -1;
            var entity = this.commentRepository.GetById(invalidId);

            Assert.IsNull(entity);
        }

        [TestMethod]
        public void GetByIdTest_ValidId_ShouldReturnValidEntity()
        {
            Comment entity = new Comment()
            {
                Content = "Test content",
                DateCreated = DateTime.Now
            };

            this.commentRepository.Add(entity);
            var foundEntity = this.commentEntities.Find(entity.Id);

            Assert.AreSame(entity, foundEntity);
        }

        [TestMethod]
        public void UpdateTest_InvalidEntityId()
        {
            int invalidId = -1;
            Comment entity = new Comment()
            {
                Content = "Test content",
                DateCreated = DateTime.Now
            };

            this.commentRepository.Add(entity);

            Comment commentToUpgradeWith = new Comment()
            {
                Content = "Updated test content",
                DateCreated = DateTime.Now.AddDays(1)
            };

            Comment upgradedComment = this.commentRepository.Update(invalidId, commentToUpgradeWith);

            Assert.IsNull(upgradedComment);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateTest_InvalidUpdateEntity_ShouldThrowArgumentNullException()
        {
            Comment entity = new Comment()
            {
                Content = "Test content",
                DateCreated = DateTime.Now
            };

            this.commentRepository.Add(entity);

            Comment commentToUpgradeWith = null;

            Comment upgradedComment = this.commentRepository.Update(entity.Id, commentToUpgradeWith);
        }

        [TestMethod]
        public void UpdateTest_ValidUpdate()
        {
            Comment entity = new Comment()
            {
                Content = "Test content",
                DateCreated = DateTime.Now
            };

            this.commentRepository.Add(entity);

            Comment commentToUpgradeWith = new Comment()
            {
                Content = "Updated test content",
                DateCreated = DateTime.Now.AddDays(1)
            };

            Comment upgradedComment = this.commentRepository.Update(entity.Id, commentToUpgradeWith);

            Assert.AreSame(entity, upgradedComment);
            Assert.AreEqual(upgradedComment.Content, commentToUpgradeWith.Content);
        }
    }
}
