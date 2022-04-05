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
    public class GenresControllerTests
    {
        private Mock<IGenreService> mockGenreService;
        private GenresController genreController;

        [SetUp]
        public void Init()
        {
            mockGenreService = new Mock<IGenreService>();
            genreController = new GenresController(mockGenreService.Object);
        }

        [Test]
        public async Task Should_ReturnOk_When_AddingValidGenre()
        {
            mockGenreService.Setup(x => x.AddGenreAsync(It.IsAny<Genre>())).ReturnsAsync(new GenreOutput());

            var result = await genreController.AddGenre(new Genre());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(OkObjectResult), result);
            Assert.AreEqual(200, ((OkObjectResult)result).StatusCode);
        }

        [Test]
        public async Task Should_ReturnBadRequest_When_AddingExistingGenre()
        {
            mockGenreService.Setup(x => x.AddGenreAsync(It.IsAny<Genre>())).ThrowsAsync(new ArgumentException());

            var result = await genreController.AddGenre(new Genre());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
            Assert.AreEqual(400, ((BadRequestObjectResult)result).StatusCode);
        }

        [Test]
        public async Task Should_ReturnOk_When_UpdatingValidGenre()
        {
            mockGenreService.Setup(x => x.UpdateGenreAsync(It.IsAny<Guid>(), It.IsAny<Genre>())).ReturnsAsync(new GenreOutput());

            var result = await genreController.UpdateGenre(Guid.NewGuid(), new Genre());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(OkObjectResult), result);
            Assert.AreEqual(200, ((OkObjectResult)result).StatusCode);
        }

        [Test]
        public async Task Should_ReturnBadRequest_When_UpdatingToAlreadyExistingGenre()
        {
            mockGenreService.Setup(x => x.UpdateGenreAsync(It.IsAny<Guid>(), It.IsAny<Genre>())).ThrowsAsync(new ArgumentException());

            var result = await genreController.UpdateGenre(Guid.NewGuid(), new Genre());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
            Assert.AreEqual(400, ((BadRequestObjectResult)result).StatusCode);
        }

        [Test]
        public async Task Should_ReturnNotFound_When_UpdatingInvalidGenre()
        {
            mockGenreService.Setup(x => x.UpdateGenreAsync(It.IsAny<Guid>(), It.IsAny<Genre>())).ThrowsAsync(new NullReferenceException());

            var result = await genreController.UpdateGenre(Guid.NewGuid(), new Genre());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(NotFoundObjectResult), result);
            Assert.AreEqual(404, ((NotFoundObjectResult)result).StatusCode);
        }

        [Test]
        public async Task Should_ReturnOk_When_DeletingExistingGenre()
        {
            mockGenreService.Setup(x => x.DeleteGenreAsync(It.IsAny<Guid>())).Returns(Task.FromResult(Guid.NewGuid()));

            var result = await genreController.DeleteGenre(Guid.NewGuid());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(OkResult), result);
            Assert.AreEqual(200, ((OkResult)result).StatusCode);
        }

        [Test]
        public async Task Should_ReturnNotFound_When_DeletingNonExistingGenre()
        {
            mockGenreService.Setup(x => x.DeleteGenreAsync(It.IsAny<Guid>())).ThrowsAsync(new NullReferenceException());

            var result = await genreController.DeleteGenre(Guid.NewGuid());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(NotFoundObjectResult), result);
            Assert.AreEqual(404, ((NotFoundObjectResult)result).StatusCode);
        }

        [Test]
        public async Task Should_ReturnBadRequest_When_DeletingAssignedGenre()
        {
            mockGenreService.Setup(x => x.DeleteGenreAsync(It.IsAny<Guid>())).ThrowsAsync(new ArgumentException());

            var result = await genreController.DeleteGenre(Guid.NewGuid());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
            Assert.AreEqual(400, ((BadRequestObjectResult)result).StatusCode);
        }

        [Test]
        public async Task Should_ReturnOk_When_GettingValidGenre()
        {
            mockGenreService.Setup(x => x.GetGenreByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new GenreOutput());

            var result = await genreController!.GetGenre(Guid.NewGuid());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(OkObjectResult), result);
            Assert.AreEqual(200, ((OkObjectResult)result).StatusCode);
        }

        [Test]
        public async Task Should_ReturnNotFound_When_GettingNonExistingGenre()
        {
            mockGenreService.Setup(x => x.GetGenreByIdAsync(It.IsAny<Guid>())).ThrowsAsync(new NullReferenceException());

            var result = await genreController!.GetGenre(Guid.NewGuid());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(NotFoundObjectResult), result);
            Assert.AreEqual(404, ((NotFoundObjectResult)result).StatusCode);
        }

        [Test]
        public async Task Should_ReturnOk_When_GettingValidAllGenresPaged()
        {
            mockGenreService.Setup(x => x.GetGenresAsync(It.IsAny<PaginatorInputDto>())).ReturnsAsync((new List<GenreOutput>(), new int()));

            var result = await genreController!.GetGenres(new PaginatorInputDto());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(OkObjectResult), result);
            Assert.AreEqual(200, ((OkObjectResult)result).StatusCode);
        }

        [Test]
        public async Task Should_ReturnNotFound_When_GettingNoGenresPaged()
        {
            mockGenreService.Setup(x => x.GetGenresAsync(It.IsAny<PaginatorInputDto>())).ThrowsAsync(new NullReferenceException());

            var result = await genreController!.GetGenres(new PaginatorInputDto());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(NotFoundObjectResult), result);
            Assert.AreEqual(404, ((NotFoundObjectResult)result).StatusCode);
        }

        [Test]
        public async Task Should_ReturnOk_When_GettingValidAllGenres()
        {
            mockGenreService.Setup(x => x.GetAllGenresAsync()).ReturnsAsync(new List<GenreOutput>());

            var result = await genreController!.GetAllGenres();

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(OkObjectResult), result);
            Assert.AreEqual(200, ((OkObjectResult)result).StatusCode);
        }

        [Test]
        public async Task Should_ReturnNotFound_When_GettingNoGenres()
        {
            mockGenreService.Setup(x => x.GetAllGenresAsync()).ThrowsAsync(new NullReferenceException());

            var result = await genreController!.GetAllGenres();

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(NotFoundObjectResult), result);
            Assert.AreEqual(404, ((NotFoundObjectResult)result).StatusCode);
        }

        [Test]
        public async Task Should_ReturnOk_When_SearchingValidGenresPaged()
        {
            mockGenreService.Setup(x => x.SearchForGenresAsync(It.IsAny<SearchGenreDto>(), It.IsAny<PaginatorInputDto>())).ReturnsAsync((new List<GenreOutput>(), new int()));

            var result = await genreController!.SearchGenres(new SearchGenreDto(), new PaginatorInputDto());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(OkObjectResult), result);
            Assert.AreEqual(200, ((OkObjectResult)result).StatusCode);
        }

        [Test]
        public async Task Should_ReturnNotFound_When_SearchingNoAuthorsPaged()
        {
            mockGenreService.Setup(x => x.SearchForGenresAsync(It.IsAny<SearchGenreDto>(), It.IsAny<PaginatorInputDto>())).ThrowsAsync(new NullReferenceException());

            var result = await genreController!.SearchGenres(new SearchGenreDto(), new PaginatorInputDto());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(NotFoundObjectResult), result);
            Assert.AreEqual(404, ((NotFoundObjectResult)result).StatusCode);
        }

        [Test]
        public async Task Should_ReturnNumber_When_GettingBooksNumberForGenre()
        {
            mockGenreService.Setup(x => x.GetBooksNumberForGenreAsync(It.IsAny<Guid>())).ReturnsAsync(new int());

            var result = await genreController!.GetGenreBooksNumber(It.IsAny<Guid>());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(OkObjectResult), result);
            Assert.AreEqual(200, ((OkObjectResult)result).StatusCode);
        }
    }
}
