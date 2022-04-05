using API.Controllers;
using Common.Models.InputDTOs;
using Common.Models.OutputDtos;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Tests.ControllersTests
{
    [TestFixture]
    public class HomeControllerTests
    {
        private readonly Mock<IBookService> mockBooksService = new();
        private readonly Mock<IGenreService> mockGenreService = new();
        private readonly Mock<IUserService> mockUserService = new();
        private HomeController? homeController;

        private LastBooksOutput lastBooksOutput = new();
        //private BookEntity bookEntity = new();

        [SetUp]
        public void Init()
        {
            homeController = new HomeController(mockBooksService.Object, mockGenreService.Object, mockUserService.Object);

            this.lastBooksOutput = new LastBooksOutput()
            {
                Id = Guid.NewGuid(),
                Title = "Book-1",
                ImageAddress = "addressFromBlob-1",
            };
        }

        [Test]
        public async Task GetLastBooks_ShouldReturnOkWhenGettingValidBooksPaged()
        {
            // Arrange
            mockBooksService.Setup(x => x.GetBooksForLastTwoWeeksAsync(It.IsAny<PaginatorInputDto>())).ReturnsAsync((new List<LastBooksOutput>(), 0));

            // Act
            var result = await homeController!.GetLastBooks(new PaginatorInputDto());

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(OkObjectResult), result);
            Assert.AreEqual(200, ((OkObjectResult)result).StatusCode);
        }

        [Test]
        public async Task GetBooksCount_ShouldReturnOkWhenGettingCountOfAllBooks()
        {
            // Arrange
            mockBooksService.Setup(x => x.GetCountOfAllBooksAsync()).ReturnsAsync(3);

            // Act
            var result = await homeController!.GetBooksCount();
            
            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(OkObjectResult), result);
            Assert.AreEqual(200, ((OkObjectResult)result).StatusCode);
            Assert.AreEqual(3, ((OkObjectResult)result).Value);
        }

        [Test]
        public async Task GetGenresCount_ShouldReturnOkWhenGettingCountOfAllGenres()
        {
            // Arrange
            mockGenreService.Setup(x => x.GetCountOfAllGenresAsync()).ReturnsAsync(3);

            // Act
            var result = await homeController!.GetGenresCount();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(OkObjectResult), result);
            Assert.AreEqual(200, ((OkObjectResult)result).StatusCode);
            Assert.AreEqual(3, ((OkObjectResult)result).Value);
        }

        [Test]
        public async Task GetReadersCount_ShouldReturnOkWhenGettingCountOfAllReaders()
        {
            // Arrange
            mockUserService.Setup(x => x.GetCountOfAllReadersAsync()).ReturnsAsync(3);

            // Act
            var result = await homeController!.GetReadersCount();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(OkObjectResult), result);
            Assert.AreEqual(200, ((OkObjectResult)result).StatusCode);
            Assert.AreEqual(3, ((OkObjectResult)result).Value);
        }
    }
}
