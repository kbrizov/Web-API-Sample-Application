using System;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using ForumDb.Data;
using ForumDb.Models;

namespace ForumDb.Repositories.Tests
{
    [TestClass]
    public class DbVoteRepositoryTests
    {
        private DbContext dbContext;
        private DbSet<Vote> voteEntities;
        private IRepository<Vote> voteRepository;
        private static TransactionScope transactionScope;

        public DbVoteRepositoryTests()
        {
            this.dbContext = new ForumDbContext();
            this.voteRepository = new DbVoteRepository(this.dbContext);
            this.voteEntities = this.dbContext.Set<Vote>();
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
            Vote entity = null;
            this.voteRepository.Add(entity);
        }

        [TestMethod]
        public void AddTest_EntityIsValid_ShouldBeAddedSuccessfuly()
        {
            Vote entity = new Vote()
            {
                Value = 1
            };

            this.voteRepository.Add(entity);
            Vote foundEntity = this.voteEntities.Find(entity.Id);

            Assert.IsTrue(entity.Id != 0);
            Assert.IsNotNull(foundEntity);
            Assert.AreSame(entity, foundEntity);
        }

        [TestMethod]
        public void GetByIdTest_InvalidId_ShouldReturnNull()
        {
            int invalidId = -1;
            var entity = this.voteRepository.GetById(invalidId);

            Assert.IsNull(entity);
        }

        [TestMethod]
        public void GetByIdTest_ValidId_ShouldReturnValidEntity()
        {
            Vote entity = new Vote()
            {
                Value = 1
            };

            this.voteRepository.Add(entity);
            Vote foundEntity = this.voteEntities.Find(entity.Id);

            Assert.AreSame(entity, foundEntity);
        }

        [TestMethod]
        public void UpdateTest_InvalidEntityId()
        {
            int invalidId = -1;
            Vote entity = new Vote()
            {
                Value = 1
            };

            this.voteRepository.Add(entity);

            Vote voteToUpdateWith = new Vote()
            {
                Value = 2
            };

            Vote updatedVote = this.voteRepository.Update(invalidId, voteToUpdateWith);

            Assert.IsNull(updatedVote);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateTest_InvalidUpdateEntity_ShouldThrowArgumentNullException()
        {
            Vote entity = new Vote()
            {
                Value = 1
            };

            this.voteRepository.Add(entity);

            Vote voteToUpdateWith = null;

            Vote updatedUser = this.voteRepository.Update(entity.Id, voteToUpdateWith);
        }

        [TestMethod]
        public void UpdateTest_ValidUpdate()
        {
            Vote entity = new Vote()
            {
                Value = 1
            };

            this.voteRepository.Add(entity);

            Vote voteToUpdateWith = new Vote()
            {
                Value = 2
            };


            Vote updatedVote = this.voteRepository.Update(entity.Id, voteToUpdateWith);

            Assert.AreSame(entity, updatedVote);
            Assert.AreEqual(updatedVote.Value, voteToUpdateWith.Value);
        }
    }
}
