using API.Controllers;
using Common.Models.InputDTOs;
using Common.Models.OutputDtos;
using DataAccess.Entities;
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
    public class BookReservationsControllerTests
    {
        private Mock<IBookReservationService>? mockBookReservationService;
        private BookReservationsController? booksReservationsController;

        [SetUp]
        public void SetUp()
        {
            mockBookReservationService = new Mock<IBookReservationService>();
            booksReservationsController = new BookReservationsController(this.mockBookReservationService.Object);
        }

        [Test]
        public async Task Should_ReturnOk_When_AddingValidBookReservation()
        {
            mockBookReservationService!.Setup(x => x.AddBookReservationAsync(It.IsAny<BookReservationDto>())).ReturnsAsync(new BookReservationEntity());

            var result = await booksReservationsController!.AddReservationBook(new BookReservationDto());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(OkObjectResult), result);
            Assert.AreEqual(200, ((OkObjectResult)result).StatusCode);
        }

        [Test]
        public async Task Should_ReturnBadRequest_When_AddingInValidBookReservation()
        {
            mockBookReservationService!.Setup(x => x.AddBookReservationAsync(It.IsAny<BookReservationDto>())).ThrowsAsync(new ArgumentException());

            var result = await booksReservationsController!.AddReservationBook(new BookReservationDto());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(BadRequestObjectResult), result);
            Assert.AreEqual(400, ((BadRequestObjectResult)result).StatusCode);
        }

        [Test]
        public async Task Should_ReturnOk_When_GettingAllUnConfirmedBookReservation()
        {
            mockBookReservationService!.Setup(x => x.GetBooksReservationsAsync(It.IsAny<PaginatorInputDto>())).ReturnsAsync((new List<BookReservationOutput>(), 35));

            var result = await booksReservationsController!.GetAllBookReservations(new PaginatorInputDto());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(OkObjectResult), result);
            Assert.AreEqual(200, ((OkObjectResult)result).StatusCode);
        }

        [Test]
        public async Task Should_ReturnNotFound_When_GettingNoUnConfirmedBookReservation()
        {
            mockBookReservationService!.Setup(x => x.GetBooksReservationsAsync(It.IsAny<PaginatorInputDto>())).ThrowsAsync(new NullReferenceException());

            var result = await booksReservationsController!.GetAllBookReservations(new PaginatorInputDto());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(NotFoundObjectResult), result);
            Assert.AreEqual(404, ((NotFoundObjectResult)result).StatusCode);
        }

        [Test]
        public async Task Should_ReturnOk_When_GetBookReservation()
        {
            mockBookReservationService.Setup(x => x.GetBookReservationByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new BookConfirmReservationOutput());

            var result = await booksReservationsController.GetBookReservation(Guid.NewGuid());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(OkObjectResult), result);
            Assert.AreEqual(200, ((OkObjectResult)result).StatusCode);
        }

        [Test]
        public async Task Should_ReturnNullReferenceException_When_GetNoBookReservation()
        {
            mockBookReservationService.Setup(x => x.GetBookReservationByIdAsync(It.IsAny<Guid>())).ThrowsAsync(new NullReferenceException());

            var result = await booksReservationsController.GetBookReservation(Guid.NewGuid());

            Assert.IsNotNull(result);
            Assert.IsInstanceOf(typeof(NotFoundObjectResult), result);
            Assert.AreEqual(404, ((NotFoundObjectResult)result).StatusCode);
        }

        [Test]
        public async Task Should_ReturnOk_When_RejectSuccessfullyBookReservation()
        {
            mockBookReservationService.Setup(x => x.RejectBookReservationByIdAsync(It.IsAny<BookReservationMessageDto>()));

            var result = await booksReservationsController.RejectReservationBook(new BookReservationMessageDto());

            Assert.IsNotNull(result);
            Assert.AreEqual(200, ((OkResult)result).StatusCode);
        }

        [Test]
        public async Task Should_ReturnInvalidOperationException_When_CanNotRejectBookReservation()
        {
            var userId = Guid.NewGuid();
            mockBookReservationService.Setup(x => x.RejectBookReservationByIdAsync(It.IsAny<BookReservationMessageDto>()))
                                                   .ThrowsAsync(new InvalidOperationException());

            var result = await booksReservationsController.RejectReservationBook(new BookReservationMessageDto() { bookReservationId=userId,librarianId=userId});

            Assert.IsNotNull(result);
            Assert.AreEqual(400, ((BadRequestObjectResult)result).StatusCode);
        }

        [Test]
        public async Task Should_ReturnNullReferenceException_When_CanNotRejectBookReservation()
        {
            mockBookReservationService.Setup(x => x.RejectBookReservationByIdAsync(It.IsAny<BookReservationMessageDto>()))
                                                   .ThrowsAsync(new NullReferenceException());

            var result = await booksReservationsController.RejectReservationBook(new BookReservationMessageDto());

            Assert.IsNotNull(result);
            Assert.AreEqual(404, ((NotFoundObjectResult)result).StatusCode);
        }

        [Test]
        public async Task Shouuld_ReturnOK_When_Approve_BookReservation()
        {
            mockBookReservationService!.Setup(x => x.ApproveBookReservation(It.IsAny<BookReservationMessageDto>()));

            var result = await booksReservationsController!.ApproveReservation(It.IsAny<BookReservationMessageDto>());

            Assert.IsNotNull(result);
            Assert.AreEqual(200, ((OkResult)result).StatusCode);
        }

        [Test]
        public async Task Should_ReturnBadRequest_When_Approve_ThrowsArgumentNullException()
        {
            mockBookReservationService!.Setup(x => x.ApproveBookReservation(It.IsAny<BookReservationMessageDto>()))
                .ThrowsAsync(new ArgumentNullException());

            var result = await booksReservationsController!.ApproveReservation(It.IsAny<BookReservationMessageDto>());

            Assert.IsNotNull(result);
            Assert.AreEqual(400, ((BadRequestObjectResult)result).StatusCode);
        }

        [Test]
        public async Task Should_ReturnBadRequest_When_Approve_ThrowsInvalidOperationException()
        {
            mockBookReservationService!.Setup(x => x.ApproveBookReservation(It.IsAny<BookReservationMessageDto>()))
                .ThrowsAsync(new InvalidOperationException());

            var result = await booksReservationsController!.ApproveReservation(It.IsAny<BookReservationMessageDto>());

            Assert.IsNotNull(result);
            Assert.AreEqual(404, ((NotFoundObjectResult)result).StatusCode);
        }

        [Test]
        public async Task Should_ReturnBadRequest_When_Approve_ThrowsNullReferenceException()
        {
            mockBookReservationService!.Setup(x => x.ApproveBookReservation(It.IsAny<BookReservationMessageDto>()))
                .ThrowsAsync(new NullReferenceException());

            var result = await booksReservationsController!.ApproveReservation(It.IsAny<BookReservationMessageDto>());

            Assert.IsNotNull(result);
            Assert.AreEqual(404, ((NotFoundObjectResult)result).StatusCode);
        }
    }
}
