using System;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using ForumDb.Data;
using ForumDb.Models;

namespace ForumDb.Repositories.Tests
{
    /// <summary>
    /// Since the EfRepository class is generic, the entity model class used doesnt matter.
    /// Category entities are used for the testing.
    /// </summary>

    [TestClass]
    public class EfRepositoryTests
    {
        private DbContext dbContext;
        private DbSet<Category> entities;
        private IRepository<Category> repository;
        private static TransactionScope transactionScope;

        public EfRepositoryTests()
        {
            this.dbContext = new ForumDbContext();
            this.repository = new EfRepository<Category>(this.dbContext);
            this.entities = this.dbContext.Set<Category>();
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
            Category entity = null;
            this.repository.Add(entity);
        }

        [TestMethod]
        public void AddTest_EntityIsValid_ShouldBeAddedSuccessfuly()
        {
            var entity = new Category()
            {
                Name = "Valid test category"
            };

            this.repository.Add(entity);
            var foundEntity = this.entities.Find(entity.Id);

            Assert.IsTrue(entity.Id != 0);
            Assert.IsNotNull(foundEntity);
            Assert.AreSame(entity, foundEntity);
        }

        [TestMethod]
        public void GetByIdTest_InvalidId_ShouldReturnNull()
        {
            int invalidId = -1;
            var entity = this.repository.GetById(invalidId);

            Assert.IsNull(entity);
        }

        [TestMethod]
        public void GetByIdTest_ValidId_ShouldReturnValidEntity()
        {
            var entity = new Category()
            {
                Name = "Test name"
            };

            this.repository.Add(entity);
            var foundEntity = this.entities.Find(entity.Id);

            Assert.AreSame(entity, foundEntity);
        }
    }
}
