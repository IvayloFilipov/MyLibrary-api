using Common.Models.InputDTOs;
using DataAccess;
using DataAccess.Entities;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests.RepositoryTests
{
    public class GenericRepositoryTests
    {
        private LibraryDbContext inMemoryContext;
        private TestRepository testRepository;
        private List<UserEntity>? users;

        [SetUp]
        public void Setup()
        {
            inMemoryContext = InMemoryDbContext.GetInMemoryDbContext("InMemoryDb");
            inMemoryContext.Database.EnsureDeleted();
            testRepository = new TestRepository(inMemoryContext);

            users = new List<UserEntity>()
            {
                new UserEntity(){FirstName = "Pesho", LastName = "Peshev"},
                new UserEntity(){FirstName = "Gosho", LastName = "Goshev"},
                new UserEntity(){FirstName = "Nikolay", LastName = "Nikolaev"},
                new UserEntity(){FirstName = "Tosho", LastName = "Toshev"}
            };

            inMemoryContext.Users.AddRange(users);
            inMemoryContext.SaveChanges();
        }

        [Test]
        public async Task GetByIdReturnTheCorrectEntity()
        {
            var userId = inMemoryContext.Users.FirstOrDefault(u => u.FirstName == "Gosho")?.Id;
            var user = await testRepository.GetByIdAsync(userId!);

            Assert.IsNotNull(user);
            Assert.That(user?.FirstName == "Gosho");
        }
      
        [Test]
        public async Task InsertWorksCorrectly()
        {
            int countBeforeInsert = inMemoryContext.Users.Count();
            var user = new UserEntity() { FirstName = "Nikolay", LastName = "Petkov" };
            await testRepository.InsertAsync(user);
            await testRepository.SaveAsync();

            int countAfterInsert = inMemoryContext.Users.Count();

            Assert.That(countBeforeInsert == countAfterInsert - 1);
        }

        [Test]
        public async Task UpdateWorksCorectly()
        {
            var user = inMemoryContext.Users.FirstOrDefault();
            user.LastName = "Angelov";

            await testRepository.UpdateAsync(user);
            await testRepository.SaveAsync();

            var updatedUser = inMemoryContext.Users.FirstOrDefault();

            Assert.That(updatedUser.FirstName == "Pesho" && updatedUser.LastName == "Angelov");
        }

        [Test]
        public async Task DeleteWorksCorrectly()
        {
            int countBeforeDelete = inMemoryContext.Users.Count();
            var id = inMemoryContext.Users.FirstOrDefault(u => u.LastName == "Nikolaev")?.Id;

            await testRepository.DeleteAsync(id!);
            await testRepository.SaveAsync();

            int countAfterDelete = inMemoryContext.Users.Count();

            Assert.That(countBeforeDelete == countAfterDelete + 1);
        }

        [Test]
        public async Task Should_ReturnList_When_GettingFirstPage()
        {
            PaginatorInputDto pager = new PaginatorInputDto
            {
                Page = 1,
                PageSize = 2
            };

            int expectedCount = 4;

            var (resultEntities, resultCount) = await testRepository.GetEntityPageAsync(pager);

            Assert.AreEqual(expectedCount, resultCount);
            Assert.AreEqual(users![0].FirstName, resultEntities[0].FirstName);
            Assert.AreEqual(users![0].LastName, resultEntities[0].LastName);
            Assert.AreEqual(users![1].FirstName, resultEntities[1].FirstName);
            Assert.AreEqual(users![1].LastName, resultEntities[1].LastName);
        }

        [Test]
        public async Task Should_ReturnList_When_GettingSecondPage()
        {
            PaginatorInputDto pager = new PaginatorInputDto
            {
                Page = 2,
                PageSize = 2
            };

            int expectedCount = 4;

            var (resultEntities, resultCount) = await testRepository.GetEntityPageAsync(pager);

            Assert.AreEqual(expectedCount, resultCount);
            Assert.AreEqual(users![2].FirstName, resultEntities[0].FirstName);
            Assert.AreEqual(users![2].LastName, resultEntities[0].LastName);
            Assert.AreEqual(users![3].FirstName, resultEntities[1].FirstName);
            Assert.AreEqual(users![3].LastName, resultEntities[1].LastName);
        }
    }
}