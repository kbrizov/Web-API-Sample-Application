using System;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using ForumDb.Data;
using ForumDb.Models;

namespace ForumDb.Repositories.Tests
{
    [TestClass]
    public class DbCategoryRepositoryTests
    {
        private DbContext dbContext;
        private DbSet<Category> categoryEntities;
        private IRepository<Category> categoryRepository;
        private static TransactionScope transactionScope;

        public DbCategoryRepositoryTests()
        {
            this.dbContext = new ForumDbContext();
            this.categoryRepository = new DbCategoryRepository(this.dbContext);
            this.categoryEntities = this.dbContext.Set<Category>();
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
            this.categoryRepository.Add(entity);
        }

        [TestMethod]
        public void AddTest_EntityIsValid_ShouldBeAddedSuccessfuly()
        {
            var entity = new Category()
            {
                Name = "Valid test category"
            };

            this.categoryRepository.Add(entity);
            var foundEntity = this.categoryEntities.Find(entity.Id);

            Assert.IsTrue(entity.Id != 0);
            Assert.IsNotNull(foundEntity);
            Assert.AreSame(entity, foundEntity);
        }

        [TestMethod]
        public void GetByIdTest_InvalidId_ShouldReturnNull()
        {
            int invalidId = -1;
            var entity = this.categoryRepository.GetById(invalidId);

            Assert.IsNull(entity);
        }

        [TestMethod]
        public void GetByIdTest_ValidId_ShouldReturnValidEntity()
        {
            var entity = new Category()
            {
                Name = "Test name"
            };

            this.categoryRepository.Add(entity);
            var foundEntity = this.categoryEntities.Find(entity.Id);

            Assert.AreSame(entity, foundEntity);
        }

        [TestMethod]
        public void UpdateTest_InvalidEntityId()
        {
            int invalidId = -1;
            Category entity = new Category()
            {
                Name = "Test category"
            };

            this.categoryRepository.Add(entity);

            Category categoryToUpgradeWith = new Category()
            {
                Name = "Updated category"
            };

            Category upgradedCategory = this.categoryRepository.Update(invalidId, categoryToUpgradeWith);

            Assert.IsNull(upgradedCategory);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateTest_InvalidUpdateEntity_ShouldThrowArgumentNullException()
        {
            Category entity = new Category()
            {
                Name = "Test category"
            };

            this.categoryRepository.Add(entity);

            Category categoryToUpgradeWith = null;

            Category upgradedCategory = this.categoryRepository.Update(entity.Id, categoryToUpgradeWith);
        }

        [TestMethod]
        public void UpdateTest_ValidUpdate()
        {
            Category entity = new Category()
            {
                Name = "Test category"
            };

            this.categoryRepository.Add(entity);

            Category categoryToUpgradeWith = new Category()
            {
                Name = "Updated category"
            };

            Category upgradedCategory = this.categoryRepository.Update(entity.Id, categoryToUpgradeWith);

            Assert.AreSame(entity, upgradedCategory);
            Assert.AreEqual(upgradedCategory.Name, categoryToUpgradeWith.Name);
        }
    }
}
