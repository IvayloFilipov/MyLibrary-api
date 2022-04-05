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
    public class GenresBooksServiceTests
    {
        private readonly Mock<IGenresBooksRepository> mockGenresBooksRepository = new Mock<IGenresBooksRepository>();
        private GenresBooksService? genresBooksService;

        [SetUp]
        public void Init()
        {
            genresBooksService = new GenresBooksService(mockGenresBooksRepository.Object);
        }

        [Test]
        public async Task Should_ReturnNumber_When_GettingBooksNumberForGenre()
        {
            mockGenresBooksRepository.Setup(x => x.GetBooksNumberForGenreAsync(It.IsAny<Guid>())).ReturnsAsync(new int());

            var result = await genresBooksService!.GetBooksNumberForGenreAsync(It.IsAny<Guid>());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(int), result);
        }
    }
}
