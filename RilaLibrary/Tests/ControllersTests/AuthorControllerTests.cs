using API.Controllers;
using Common.Models.InputDTOs;
using Common.Models.OutputDtos;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.ControllersTests
{
    [TestFixture]
    public class AuthorControllerTests
    {
        private Mock<IAuthorService> mockAuthorService;
        private AuthorsController authorsController;
        private AuthorDto input;

        [SetUp]
        public void SetUp()
        {
            mockAuthorService = new Mock<IAuthorService>();
            authorsController = new AuthorsController(mockAuthorService.Object);

            input = new AuthorDto() { AuthorName = "Mark Twain" };
        }

        [Test]
        public async Task AddReturnsOK()
        {
            mockAuthorService.Setup(x => x.AddAuthorAsync(It.IsAny<AuthorDto>())).ReturnsAsync(new AuthorOutput());

            var output = await authorsController.AddAuthor(input);

            Assert.IsTrue(output is not null);
            Assert.AreEqual(200, ((OkObjectResult)output!).StatusCode);
        }

        [Test]
        public async Task AddReturnsBadRequest()
        {
            mockAuthorService.Setup(x => x.AddAuthorAsync(It.IsAny<AuthorDto>())).ThrowsAsync(new ArgumentException());

            var output = await authorsController.AddAuthor(input);

            Assert.IsTrue(output is not null);
            Assert.AreEqual(400, ((BadRequestObjectResult)output!).StatusCode);
        }

        [Test]
        public async Task GetAllReturnsOK()
        {
            mockAuthorService.Setup(x => x.GetAllAuthorsAsync()).ReturnsAsync(new List<AuthorOutput>());

            var output = await authorsController.GetAll();

            Assert.IsTrue(output is not null);
            Assert.AreEqual(200, ((OkObjectResult)output!).StatusCode);
        }

        [Test]
        public async Task GetAllReturnsBadRequest()
        {
            mockAuthorService.Setup(x => x.GetAllAuthorsAsync()).ThrowsAsync(new NullReferenceException());

            var output = await authorsController.GetAll();

            Assert.IsTrue(output is not null);
            Assert.AreEqual(404, ((NotFoundObjectResult)output!).StatusCode);
        }

        [Test]
        public async Task GetAuthorsReturnsOK()
        {
            mockAuthorService.Setup(x => x.GetAuthorsAsync(It.IsAny<PaginatorInputDto>())).ReturnsAsync((new List<AuthorOutput>(), 35));

            var output = await authorsController.GetAuthors(new PaginatorInputDto());

            Assert.IsTrue(output is not null);
            Assert.AreEqual(200, ((OkObjectResult)output!).StatusCode);
        }

        [Test]
        public async Task GetAuthorsReturnsBadRequest()
        {
            mockAuthorService.Setup(x => x.GetAuthorsAsync(It.IsAny<PaginatorInputDto>())).ThrowsAsync(new NullReferenceException());

            var output = await authorsController.GetAuthors(new PaginatorInputDto());

            Assert.IsTrue(output is not null);
            Assert.AreEqual(404, ((NotFoundObjectResult)output!).StatusCode);
        }

        [Test]
        public async Task GetAuthorByIdReturnsOK()
        {
            mockAuthorService.Setup(x => x.GetAuthorByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new AuthorOutput());

            var output = await authorsController.GetAuthor(Guid.NewGuid());

            Assert.IsTrue(output is not null);
            Assert.AreEqual(200, ((OkObjectResult)output!).StatusCode);
        }

        [Test]
        public async Task GetAuthorByIdReturnsBadRequest()
        {
            mockAuthorService.Setup(x => x.GetAuthorByIdAsync(It.IsAny<Guid>())).ThrowsAsync(new NullReferenceException());

            var output = await authorsController.GetAuthor(Guid.NewGuid());

            Assert.IsTrue(output is not null);
            Assert.AreEqual(404, ((NotFoundObjectResult)output!).StatusCode);
        }

        [Test]
        public async Task UpdateAuthorReturnsOK()
        {
            mockAuthorService.Setup(x => x.UpdateAuthorAsync(It.IsAny<AuthorDto>(), It.IsAny<Guid>())).ReturnsAsync(new AuthorOutput());

            var output = await authorsController.UpdateAuthor(input, Guid.NewGuid());

            Assert.IsTrue(output is not null);
            Assert.AreEqual(200, ((OkObjectResult)output!).StatusCode);
        }

        [Test]
        public async Task UpdateAuthorReturnsNotFound()
        {
            mockAuthorService.Setup(x => x.UpdateAuthorAsync(It.IsAny<AuthorDto>(), It.IsAny<Guid>())).ThrowsAsync(new NullReferenceException());

            var output = await authorsController.UpdateAuthor(input, Guid.NewGuid());

            Assert.IsTrue(output is not null);
            Assert.AreEqual(404, ((NotFoundObjectResult)output!).StatusCode);
        }

        [Test]
        public async Task UpdateAuthorReturnsBadRequest()
        {
            mockAuthorService.Setup(x => x.UpdateAuthorAsync(It.IsAny<AuthorDto>(), It.IsAny<Guid>())).ThrowsAsync(new ArgumentException());

            var output = await authorsController.UpdateAuthor(input, Guid.NewGuid());

            Assert.IsTrue(output is not null);
            Assert.AreEqual(400, ((BadRequestObjectResult)output!).StatusCode);
        }

        [Test]
        public async Task DeleteAuthorReturnsOK()
        {
            mockAuthorService.Setup(x => x.DeleteAuthorAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);

            var output = await authorsController.DeleteAuthor(Guid.NewGuid());

            Assert.IsTrue(output is not null);
            Assert.AreEqual(200, ((OkResult)output!).StatusCode);
        }

        [Test]
        public async Task DeleteAuthorReturnsBadRequest()
        {
            mockAuthorService.Setup(x => x.DeleteAuthorAsync(It.IsAny<Guid>())).ThrowsAsync(new ArgumentException());

            var output = await authorsController.DeleteAuthor(Guid.NewGuid());

            Assert.IsTrue(output is not null);
            Assert.AreEqual(400, ((BadRequestObjectResult)output!).StatusCode);
        }

        [Test]
        public async Task DeleteAuthorReturnsNotFound()
        {
            mockAuthorService.Setup(x => x.DeleteAuthorAsync(It.IsAny<Guid>())).ThrowsAsync(new NullReferenceException());

            var output = await authorsController.DeleteAuthor(Guid.NewGuid());

            Assert.IsTrue(output is not null);
            Assert.AreEqual(404, ((NotFoundObjectResult)output!).StatusCode);
        }

        [Test]
        public async Task Should_ReturnOk_When_SearchingValidAuthorsPaged()
        {
            mockAuthorService.Setup(x => x.SearchForAuthorsAsync(It.IsAny<SearchAuthorDto>(), It.IsAny<PaginatorInputDto>())).ReturnsAsync((new List<AuthorOutput>(), new int()));

            var result = await authorsController!.SearchAuthors(new SearchAuthorDto(), new PaginatorInputDto());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(OkObjectResult), result);
            Assert.AreEqual(200, ((OkObjectResult)result).StatusCode);
        }

        [Test]
        public async Task Should_ReturnNotFound_When_SearchingNoAuthorsPaged()
        {
            mockAuthorService.Setup(x => x.SearchForAuthorsAsync(It.IsAny<SearchAuthorDto>(), It.IsAny<PaginatorInputDto>())).ThrowsAsync(new NullReferenceException());

            var result = await authorsController!.SearchAuthors(new SearchAuthorDto(), new PaginatorInputDto());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(NotFoundObjectResult), result);
            Assert.AreEqual(404, ((NotFoundObjectResult)result).StatusCode);
        }

        [Test]
        public async Task Should_ReturnNumber_When_GettingBooksNumberForAuthor()
        {
            mockAuthorService.Setup(x => x.GetBooksNumberForAuthorAsync(It.IsAny<Guid>())).ReturnsAsync(new int());

            var result = await authorsController!.GetAuthorBooksNumber(It.IsAny<Guid>());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(OkObjectResult), result);
            Assert.AreEqual(200, ((OkObjectResult)result).StatusCode);
        }
    }
}
