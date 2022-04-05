using DataAccess;
using DataAccess.Entities;
using NUnit.Framework;
using Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.RepositoryTests
{
    [TestFixture]
    public class UserRepositoryTests
    {
        private LibraryDbContext? inMemoryContext;
        private UserRepository? userRepository;

        [SetUp]
        public void Init()
        {
            inMemoryContext = InMemoryDbContext.GetInMemoryDbContext("InMemoryDb");
            inMemoryContext.Database.EnsureDeleted();
            userRepository = new UserRepository(inMemoryContext);

            List<UserEntity> users = new List<UserEntity>
            {
                new UserEntity { FirstName = "John",  Id = "1117baea-311f-4387-9b9b-ef4c6ec8b5ce", Email = "em@em.com" },
                new UserEntity { FirstName = "Manuel",  Id = "2227baea-311f-4387-9b9b-ef4c6ec8b5ce", Email = "em1@em.com" },
                new UserEntity { FirstName = "Author", Id = "3337baea-311f-4387-9b9b-ef4c6ec8b5ce", Email = "em2@em.com" },
            };

            inMemoryContext.Users.AddRange(users);
            inMemoryContext.SaveChanges();
        }

        [Test]
        public async Task GetCountOfAllReadersAsync_ShouldReturnCorrectNumberOfExistingReaders()
        {
            // Arrange
            int expectedResult = 3;

            // Act
            var actualResult = await userRepository!.GetCountOfAllReadersAsync();

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}
