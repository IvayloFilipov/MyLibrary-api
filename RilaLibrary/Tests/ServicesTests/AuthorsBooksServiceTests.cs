using Common;
using Common.Models.InputDTOs;
using Common.Models.OutputDtos;
using DataAccess.Entities;
using Moq;
using NUnit.Framework;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.ServicesTests
{
    [TestFixture]
    public class AuthorsBooksServiceTests
    {
        private readonly Mock<IAuthorsBooksRepository> mockAuthorsBooksRepository = new Mock<IAuthorsBooksRepository>();
        private IAuthorsBooksService? authorsBooksService;

        [SetUp]
        public void SetUp()
        {
            authorsBooksService = new AuthorsBooksService(mockAuthorsBooksRepository.Object);
        }

        [Test]
        public async Task Should_ReturnNumber_When_GettingBooksNumberForAuthor()
        {
            mockAuthorsBooksRepository.Setup(x => x.GetBooksNumberForAuthorAsync(It.IsAny<Guid>())).ReturnsAsync(new int());

            var result = await authorsBooksService!.GetBooksNumberForAuthorAsync(It.IsAny<Guid>());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(int), result);
        }
    }
}
